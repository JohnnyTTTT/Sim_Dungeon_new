using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.Profiling.HierarchyFrameDataView;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;

namespace Johnny.SimDungeon
{

    public class BuildableObjectsPanelViewModel : ListViewModel<BuildableGenItemViewModel>
    {
        public Dictionary<BuildableObjectUICategorySO, ObservableList<BuildableGenItemViewModel>> AllItems;

        public CategoryObjectItemViewModel ActiveCategoryObjectItemView
        {
            get
            {
                return m_activeCategoryObjectItemView;
            }
            set
            {
                if (m_activeCategoryObjectItemView != value)
                {
                    Set(ref m_activeCategoryObjectItemView, value);
                    if (m_activeCategoryObjectItemView! != null)
                    {
                        SetSelectedItem(null);
                        //ClearItem();
                        if (AllItems.TryGetValue(m_activeCategoryObjectItemView.Data, out var datas))
                        {
                            Items = datas;
                            RaisePropertyChanged();
                        }
                    }
                    else
                    {
                        SetSelectedItem(null);
                    }
                }
            }
        }
        private CategoryObjectItemViewModel m_activeCategoryObjectItemView;

        public string CategoryObjectItemName
        {
            get
            {
                return ActiveCategoryObjectItemView != null ? ActiveCategoryObjectItemView.Data.categoryName : "";
            }
        }

        private MainGameViewModel m_MainGameViewModel;
        private IDisposable m_Subscription;

        public BuildableObjectsPanelViewModel(BuildableGenAssets buildableGenAssets)
        {
            var serviceContainer = Context.GetApplicationContext().GetContainer();
            m_MainGameViewModel = serviceContainer.Resolve<MainGameViewModel>();
            m_Subscription = Loxodon.Framework.Messaging.Messenger.Default.Subscribe<PropertyChangedMessage<CategoryObjectItemViewModel>>(OnCategoryObjectItemViewModelChanged);
            AllItems = new Dictionary<BuildableObjectUICategorySO, ObservableList<BuildableGenItemViewModel>>();
            CreateItems(buildableGenAssets);
        }

        private void OnCategoryObjectItemViewModelChanged(PropertyChangedMessage<CategoryObjectItemViewModel> message)
        {
            ActiveCategoryObjectItemView = message.NewValue;
        }

        protected override void OnSelectedItemChanged(BuildableGenItemViewModel old, BuildableGenItemViewModel item)
        {
            if (item != null)
            {
                SpawnManager.Instance.SetInputActiveBuildableObjectSO(item.Data.buildableObjectSO, item.Data.gridType);
            }
        }

        public void CreateItems(BuildableGenAssets buildableGenAssets)
        {
            foreach (var buildableGen in BuildableAssets.Instance.buildableGenAssets.allAssets)
            {
                var categorySO = buildableGen.buildableObjectSO.buildableObjectUICategorySO;
                if (!AllItems.ContainsKey(categorySO))
                {
                    AllItems[categorySO] = new ObservableList<BuildableGenItemViewModel>();
                }
                var item = new BuildableGenItemViewModel(buildableGen, ItemSelectCommand);
                AllItems[categorySO].Add(item);
            }
        }

    }

    public class BuildableObjectsPanelView : UIView
    {
        [SerializeField] private AnimationPanel m_AnimationPanel;
        [SerializeField] private BuildableObjectListView m_ListView;
        [SerializeField] private TextMeshProUGUI m_Title;
        private BuildableObjectsPanelViewModel m_ViewModel;

        protected override void Awake()
        {
            m_ViewModel = new BuildableObjectsPanelViewModel(BuildableAssets.Instance.buildableGenAssets);
            this.SetDataContext(m_ViewModel);

            var serviceContainer = Context.GetApplicationContext().GetContainer();
            serviceContainer.Register(m_ViewModel);
        }


        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<BuildableObjectsPanelView, BuildableObjectsPanelViewModel>();

            bindingSet.Bind(this.m_ListView).For(v => v.Items).To(vm => vm.Items).OneWay();
            bindingSet.Bind(this.m_AnimationPanel).For(v => v.Show).ToExpression(vm => vm.ActiveCategoryObjectItemView != null).OneWay();
            bindingSet.Bind(this.m_Title).For(v => v.text).To(vm => vm.CategoryObjectItemName).OneWay();

            bindingSet.Build();

            //GridManager.Instance.OnActiveGridModeChanged += OnActiveGridModeChanged;
        }

    }
}
