using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Services;
using Loxodon.Framework.Tutorials;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using NUnit.Framework;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Profiling.HierarchyFrameDataView;

namespace Johnny.SimDungeon
{
    public class CategoryObjectsPanelViewModel : ListViewModel<CategoryObjectItemViewModel>
    {
        public Dictionary<BuildCategory, ObservableList<CategoryObjectItemViewModel>> AllItems;

        public bool IsVisible
        {
            get
            {
                return m_IsVisible;
            }
            set
            {
                Set(ref m_IsVisible, value);
            }
        }
        private bool m_IsVisible;


        public BuildCategory ActiveBuildCategory
        {
            get
            {
                return m_ActiveBuildCategory;
            }
            set
            {
                if (m_ActiveBuildCategory != value)
                {
                    Set(ref m_ActiveBuildCategory, value);
                    SetSelectedItem(null);
                    if (AllItems.TryGetValue(m_ActiveBuildCategory, out var datas))
                    {
                        Items = datas;
                    }
                }
            }
        }
        private BuildCategory m_ActiveBuildCategory;

        private IDisposable m_Subscription;

        public CategoryObjectsPanelViewModel(BuildableGenAssets buildableGenAssets)
        {
            m_Subscription = Loxodon.Framework.Messaging.Messenger.Default.Subscribe<PropertyChangedMessage<GameState>>(OnGameStateChanged);
            AllItems = new Dictionary<BuildCategory, ObservableList<CategoryObjectItemViewModel>>();
            CreateItems(buildableGenAssets);
        }

        private void OnGameStateChanged(PropertyChangedMessage<GameState> message)
        {
            var value = message.NewValue;
            IsVisible = value == GameState.Structure || value == GameState.Placement;
            if (value == GameState.Structure)
            {
                IsVisible = true;
                ActiveBuildCategory = BuildCategory.Structure;
            }
            else if (value == GameState.Placement)
            {
                IsVisible = true;
                ActiveBuildCategory = BuildCategory.Placement;
            }
            else
            {
                IsVisible = false;
                ActiveBuildCategory = BuildCategory.None;
            }
        }

        protected override void OnSelectedItemChanged(CategoryObjectItemViewModel old, CategoryObjectItemViewModel item)
        {
            Loxodon.Framework.Messaging.Messenger.Default.Publish(new PropertyChangedMessage<CategoryObjectItemViewModel>(old, item, nameof(OnSelectedItemChanged)));
            //BindingService.BuildableObjectsPanelViewModel.ActiveCategoryObjectItemView = item;
        }

        public void CreateItems(BuildableGenAssets buildableGenAssets)
        {
            foreach (var buildableGen in BuildableAssets.Instance.buildableGenAssets.allAssets)
            {
                if (!AllItems.ContainsKey(buildableGen.buildCategory))
                {
                    AllItems[buildableGen.buildCategory] = new ObservableList<CategoryObjectItemViewModel>();
                }
                var item = new CategoryObjectItemViewModel(buildableGen.buildableObjectSO.buildableObjectUICategorySO, this.ItemSelectCommand);
                AllItems[buildableGen.buildCategory].Add(item);
            }
        }

    }

    public class CategoryObjectsPanelView : UIView
    {
        [SerializeField] private AnimationPanel m_AnimationPanel;
        [SerializeField] private CategoryObjectsListView m_ListView;
        private CategoryObjectsPanelViewModel m_ViewModel;
        protected override void Awake()
        {
            m_ViewModel = new CategoryObjectsPanelViewModel(BuildableAssets.Instance.buildableGenAssets);
            this.SetDataContext(m_ViewModel);

            var serviceContainer = Context.GetApplicationContext().GetContainer();
            serviceContainer.Register(m_ViewModel);
        }

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<CategoryObjectsPanelView, CategoryObjectsPanelViewModel>();

            bindingSet.Bind(this.m_AnimationPanel).For(v => v.Show).To(vm => vm.IsVisible).OneWay();
            bindingSet.Bind(this.m_ListView).For(v => v.Items).To(vm => vm.Items).OneWay();

            bindingSet.Build();
        }

        private void OnActiveGridModeChanged(EasyGridBuilderPro easyGridBuilderPro, GridMode gridMode)
        {
            if (gridMode != GridMode.BuildMode && m_ViewModel.SelectedItem != null)
            {
                m_ViewModel.SetSelectedItem(null);
            }
        }

    }
}
