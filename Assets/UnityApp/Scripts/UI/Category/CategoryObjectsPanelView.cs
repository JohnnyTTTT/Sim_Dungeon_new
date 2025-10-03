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
    public class CategoryObjectItemViewModel : SelectableItemViewModel
    {
        public BuildableObjectUICategorySO Data
        {
            get { return this.data; }
            set { this.Set(ref data, value); }
        }
        private BuildableObjectUICategorySO data;

        public CategoryObjectItemViewModel(BuildableObjectUICategorySO categorySO, Loxodon.Framework.Commands.ICommand selectCommand) :
            base(selectCommand)
        {
            if (categorySO != null)
            {
                Icon = categorySO.categoryIcon;
                Data = categorySO;
                Description = categorySO.categoryName;
            }
        }
    }

    public class CategoryObjectsPanelViewModel : ListViewModel
    {
        public Dictionary<BuildCategory, ObservableList<SelectableItemViewModel>> AllItems;

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

        public BuildCategory ActiveBuildCategory
        {
            get
            {
                return m_ActiveBuildCategory;
            }
            set
            {
                if (Set(ref m_ActiveBuildCategory, value))
                {
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
        private IDisposable m_FixedToolbarSubscription;

        public CategoryObjectsPanelViewModel(BuildableGenAssets buildableGenAssets, IMessenger messenger) : base(messenger)
        {
            m_Subscription = Messenger.Subscribe<PropertyChangedMessage<GameState>>(OnGameStateChanged);

            m_FixedToolbarSubscription = Messenger.Subscribe<PropertyChangedMessage<SelectableItemViewModel>>(
                FixedToolbarViewModel.BeforeFixedToolbarSelectedItemChange, OnFixedToolbarViewModelItemChange);

            AllItems = new Dictionary<BuildCategory, ObservableList<SelectableItemViewModel>>();
            CreateItems(buildableGenAssets);
        }

        private void OnFixedToolbarViewModelItemChange(PropertyChangedMessage<SelectableItemViewModel> message)
        {
            if (message.NewValue != null)
            {
                Debug.Log(2);
                SetSelectedItem(null);  
            }
        }

        private void OnGameStateChanged(PropertyChangedMessage<GameState> message)
        {
            var value = message.NewValue;
            if (value == GameState.Structure)
            {
                ActiveBuildCategory = BuildCategory.Structure;
                Visibility = true;
            }
            else if (value == GameState.Placement)
            {
                ActiveBuildCategory = BuildCategory.Placement;
                Visibility = true;
            }
            else
            {
                ActiveBuildCategory = BuildCategory.None;
                Visibility = false;
            }
        }

        protected override void OnSelectedItemChanged(SelectableItemViewModel old, SelectableItemViewModel item)
        {
            SpawnManager.Instance.GridModeReset();
            var oldVM = old as CategoryObjectItemViewModel;
            var newVM = item as CategoryObjectItemViewModel;
            Messenger.Publish(new PropertyChangedMessage<CategoryObjectItemViewModel>(oldVM, newVM, nameof(CategoryObjectItemViewModel)));
            //BindingService.BuildableObjectsPanelViewModel.ActiveCategoryObjectItemView = item;
        }

        public void CreateItems(BuildableGenAssets buildableGenAssets)
        {
            foreach (var buildableGen in BuildableAssets.Instance.buildableGenAssets.allAssets)
            {
                if (!AllItems.ContainsKey(buildableGen.buildCategory))
                {
                    AllItems[buildableGen.buildCategory] = new ObservableList<SelectableItemViewModel>();
                }
                var item = new CategoryObjectItemViewModel(buildableGen.buildableObjectSO.buildableObjectUICategorySO, this.ItemSelectCommand);
                AllItems[buildableGen.buildCategory].Add(item);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (m_Subscription != null)
            {
                m_Subscription.Dispose();
                m_Subscription = null;
            }
            if (m_FixedToolbarSubscription != null)
            {
                m_FixedToolbarSubscription.Dispose();
                m_FixedToolbarSubscription = null;
            }
            AllItems?.Clear();
        }

    }

    public class CategoryObjectsPanelView : AnimationUIView
    {
        [SerializeField] private UIViewPositionAnimation m_AnimationPanel;
        [SerializeField] private ListView m_ListView;
        private CategoryObjectsPanelViewModel m_ViewModel;

        protected override void Awake()
        {
            m_ViewModel = new CategoryObjectsPanelViewModel(BuildableAssets.Instance.buildableGenAssets, Messenger.Default);
            this.SetDataContext(m_ViewModel);

            var serviceContainer = Context.GetApplicationContext().GetContainer();
            serviceContainer.Register(m_ViewModel);
        }

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<CategoryObjectsPanelView, CategoryObjectsPanelViewModel>();

            //bindingSet.Bind(this.m_AnimationPanel).For(v => v.Show).To(vm => vm.IsVisible).OneWay();
            bindingSet.Bind(this).For(v => v.Visibility).To(vm => vm.Visibility).OneWay();
            bindingSet.Bind(this.m_ListView).For(v => v.Items).To(vm => vm.Items).OneWay();

            bindingSet.Build();
        }

        protected override void OnDestroy()
        {
            this.ClearAllBindings();
        }

        private void OnActiveGridModeChanged(EasyGridBuilderPro easyGridBuilderPro, GridMode gridMode)
        {
            if (gridMode != GridMode.BuildMode && m_ViewModel.GetSelectedItem() != null)
            {
                m_ViewModel.SetSelectedItem(null);
            }
        }

    }
}
