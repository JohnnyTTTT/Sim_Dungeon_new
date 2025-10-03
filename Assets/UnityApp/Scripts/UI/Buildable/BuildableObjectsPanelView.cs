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
    public class BuildableGenItemViewModel : SelectableItemViewModel
    {
        public BuildableGen Data
        {
            get { return this.data; }
            set { this.Set(ref data, value); }
        }
        private BuildableGen data;

        public BuildableGenItemViewModel(BuildableGen buildableGen, Loxodon.Framework.Commands.ICommand selectCommand) :
            base(selectCommand, null, buildableGen.buildableObjectSO.objectIcon, buildableGen.buildableObjectSO.objectName, buildableGen.description)
        {
            Data = buildableGen;
        }
    }

    public class BuildableObjectsPanelViewModel : ListViewModel<SelectableItemViewModel>
    {
        public Dictionary<BuildableObjectUICategorySO, ObservableList<SelectableItemViewModel>> AllItems;

        public CategoryObjectItemViewModel ActiveCategoryObjectItemView
        {
            get
            {
                return m_activeCategoryObjectItemView;
            }
            set
            {
                if (Set(ref m_activeCategoryObjectItemView, value))
                {
                    SetSelectedItem(null);
                    if (m_activeCategoryObjectItemView! != null)
                    {
                        if (AllItems.TryGetValue(m_activeCategoryObjectItemView.Data, out var datas))
                        {
                            Items = datas;
                            RaisePropertyChanged();
                        }
                    }
                }
            }
        }
        private CategoryObjectItemViewModel m_activeCategoryObjectItemView;

        public bool Visibility
        {
            get
            {
                return m_Visibility;
            }
            set
            {
                Set(ref m_Visibility, value);
            }
        }
        private bool m_Visibility;

        public string CategoryObjectItemName
        {
            get
            {
                return ActiveCategoryObjectItemView != null ? ActiveCategoryObjectItemView.Data.categoryName : "";
            }
        }

        private IDisposable m_Subscription;

        public BuildableObjectsPanelViewModel(BuildableGenAssets buildableGenAssets, IMessenger messenger) : base(messenger)
        {
            m_Subscription = Messenger.Subscribe<PropertyChangedMessage<CategoryObjectItemViewModel>>(OnCategoryObjectItemViewModelChanged);
            AllItems = new Dictionary<BuildableObjectUICategorySO, ObservableList<SelectableItemViewModel>>();
            CreateItems(buildableGenAssets);
        }

        private void OnCategoryObjectItemViewModelChanged(PropertyChangedMessage<CategoryObjectItemViewModel> message)
        {
            Visibility = message.NewValue != null;
            ActiveCategoryObjectItemView = message.NewValue;
        }

        protected override void OnSelectedItemChanged(SelectableItemViewModel old, SelectableItemViewModel item)
        {
            var oldVM = old as BuildableGenItemViewModel;
            var newVM = item as BuildableGenItemViewModel;
            if (item != null)
            {
                Debug.Log(newVM.Data.buildableObjectSO);
                Debug.Log(newVM.Data.gridType);
                SpawnManager.Instance.SetInputActiveBuildableObjectSO(newVM.Data.buildableObjectSO, newVM.Data.gridType);
            }
            Messenger.Publish(new PropertyChangedMessage<BuildableGenItemViewModel>(oldVM, newVM, nameof(BuildableGenItemViewModel)));
        }

        public void CreateItems(BuildableGenAssets buildableGenAssets)
        {
            foreach (var buildableGen in BuildableAssets.Instance.buildableGenAssets.allAssets)
            {
                var categorySO = buildableGen.buildableObjectSO.buildableObjectUICategorySO;
                if (!AllItems.ContainsKey(categorySO))
                {
                    AllItems[categorySO] = new ObservableList<SelectableItemViewModel>();
                }
                var item = new BuildableGenItemViewModel(buildableGen, ItemSelectCommand);
                AllItems[categorySO].Add(item);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_Subscription != null)
                {
                    m_Subscription.Dispose();
                    m_Subscription = null;
                    AllItems?.Clear();
                }
            }
        }
    }

    public class BuildableObjectsPanelView : AnimationUIView
    {
        [SerializeField] private UIViewPositionAnimation m_AnimationPanel;
        [SerializeField] private ListView m_ListView;
        [SerializeField] private TextMeshProUGUI m_Title;
        private BuildableObjectsPanelViewModel m_ViewModel;

        protected override void Awake()
        {
            m_ViewModel = new BuildableObjectsPanelViewModel(BuildableAssets.Instance.buildableGenAssets, Messenger.Default);
            this.SetDataContext(m_ViewModel);

            var serviceContainer = Context.GetApplicationContext().GetContainer();
            serviceContainer.Register(m_ViewModel);
        }


        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<BuildableObjectsPanelView, BuildableObjectsPanelViewModel>();

            bindingSet.Bind(this).For(v => v.Visibility).To(vm => vm.Visibility).OneWay();
            bindingSet.Bind(this.m_ListView).For(v => v.Items).To(vm => vm.Items).OneWay();
            bindingSet.Bind(this.m_Title).For(v => v.text).To(vm => vm.CategoryObjectItemName).OneWay();

            bindingSet.Build();

            //GridManager.Instance.OnActiveGridModeChanged += OnActiveGridModeChanged;
        }

    }
}
