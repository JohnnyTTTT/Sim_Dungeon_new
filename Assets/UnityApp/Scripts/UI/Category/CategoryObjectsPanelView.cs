using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
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
        public Dictionary<BuildCategory, ObservableList<CategoryObjectItemViewModel>> AllItems = new Dictionary<BuildCategory, ObservableList<CategoryObjectItemViewModel>>();

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
                    m_ActiveBuildCategory = value;
                    if (AllItems.TryGetValue(m_ActiveBuildCategory, out var datas))
                    {
                        Items = datas;
                    }
                }
            }
        }
        private BuildCategory m_ActiveBuildCategory;

        protected override void OnSelectedItemChanged(CategoryObjectItemViewModel old, CategoryObjectItemViewModel item)
        {
            BindingService.BuildableObjectsPanelViewModel.ActiveCategoryObjectItemView = item;
        }

        public CategoryObjectItemViewModel CreateItem(BuildableObjectUICategorySO buildableObjectUICategorySO)
        {
            var item = new CategoryObjectItemViewModel(buildableObjectUICategorySO, this.ItemSelectCommand);
            return item;
        }
    }

    public class CategoryObjectsPanelView : ViewBase<CategoryObjectsPanelViewModel>
    {
        [SerializeField] private AnimationPanel m_AnimationPanel;
        [SerializeField] private CategoryObjectsListView m_ListView;

        protected override void Start()
        {
            ViewModel = BindingService.CategoryObjectsPanelViewModel;
            base.Start();
        }

        protected override void Binding(BindingSet<ViewBase<CategoryObjectsPanelViewModel>, CategoryObjectsPanelViewModel> bindingSet)
        {
            bindingSet.Bind(this.m_ListView).For(v => v.Items).To(vm => vm.Items).OneWay();

        }

        protected override void StaticBinding(BindingSet<ViewBase<CategoryObjectsPanelViewModel>> staticBindingSet)
        {
            staticBindingSet.Bind(this.m_AnimationPanel).For(v => v.Show).To(() => BindingService.MainGameViewModel.ShouldShowCategoryUI).OneWay();
        }

        public void Init()
        {
            var allItems = ViewModel.AllItems;
            foreach (var buildableGen in BuildableAssets.Instance.buildableGenAssets.allAssets)
            {
                if (!allItems.ContainsKey(buildableGen.buildCategory))
                {
                    allItems[buildableGen.buildCategory] = new ObservableList<CategoryObjectItemViewModel>();
                }
                var item = ViewModel.CreateItem(buildableGen.buildableObjectSO.buildableObjectUICategorySO);
                allItems[buildableGen.buildCategory].Add(item);
            }
        }

        private void OnActiveBuildableSOChanged(EasyGridBuilderPro easyGridBuilderPro, BuildableObjectSO buildableObjectSO)
        {
            //if (BindingService.MainGameViewModel.ActiveEasyGridBuilderPro != easyGridBuilderPro) return;
            //if (buildableObjectSO == null && ViewModel.SelectedItem != null)
            //{
            //    ViewModel.SelectedItem = null;
            //}
        }

    }
}
