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

namespace Johnny.SimDungeon
{

    public class BuildableObjectsPanelViewModel : ListViewModel<BuildableGenItemViewModel>
    {
        public Dictionary<BuildableObjectUICategorySO, ObservableList<BuildableGenItemViewModel>> AllItems = new Dictionary<BuildableObjectUICategorySO, ObservableList<BuildableGenItemViewModel>>();

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

        private EasyGridBuilderPro GetItemGridTypeEasyGridBuilderPro(BuildableGenItemViewModel item)
        {
            switch (item.Data.gridType)
            {
                case GridType.Undefined:
                    break;
                case GridType.Nothing:
                    break;
                case GridType.Large:
                    return SpawnManager.Instance.m_EasyGridBuilderPro_LargeCell;
                case GridType.Small:
                    return SpawnManager.Instance.m_EasyGridBuilderPro_SmallCell;
                default:
                    break;
            }
            return null;
        }

        //public bool temp_Lock;

        protected override void OnSelectedItemChanged(BuildableGenItemViewModel old, BuildableGenItemViewModel item)
        {
            if (item != null)
            {
                //temp_Lock = true;
                var grid = GetItemGridTypeEasyGridBuilderPro(item);

                if (BindingService.MainGameViewModel.GridType != item.Data.gridType)
                {
                    BindingService.MainGameViewModel.GridType = item.Data.gridType;
                }
                if (grid.GetActiveGridMode() != GridMode.BuildMode)
                {
                    grid.SetActiveGridMode(GridMode.BuildMode);
                }

                grid.SetInputActiveBuildableObjectSO(item.Data.buildableObjectSO, null, true);
                //temp_Lock = false;
            }
            else
            {
                if (old != null)
                {
                    var grid = GetItemGridTypeEasyGridBuilderPro(old);
                    if (BindingService.MainGameViewModel.GridType != GridType.Nothing)
                    {
                        BindingService.MainGameViewModel.GridType = GridType.Nothing;
                    }
                    if (grid.GetActiveGridMode() != GridMode.None)
                    {
                        grid.SetActiveGridMode(GridMode.None);
                    }
                }
            }
        }

        public BuildableGenItemViewModel CreateItem(BuildableGen buildableGen)
        {
            var item = new BuildableGenItemViewModel(buildableGen, ItemSelectCommand);
            return item;
        }
    }

    public class BuildableObjectsPanelView : ViewBase<BuildableObjectsPanelViewModel>
    {
        [SerializeField] private AnimationPanel m_AnimationPanel;
        [SerializeField] private BuildableObjectListView m_ListView;
        [SerializeField] private TextMeshProUGUI m_Title;


        protected override void Start()
        {
            ViewModel = BindingService.BuildableObjectsPanelViewModel;
            //GridManager.Instance.OnActiveBuildableSOChanged += OnActiveBuildableSOChanged;
            GridManager.Instance.OnActiveGridModeChanged += OnActiveGridModeChanged;
            base.Start();
        }

        protected override void Binding(BindingSet<ViewBase<BuildableObjectsPanelViewModel>, BuildableObjectsPanelViewModel> bindingSet)
        {
            bindingSet.Bind(this.m_ListView).For(v => v.Items).To(vm => vm.Items).OneWay();
            bindingSet.Bind(this.m_AnimationPanel).For(v => v.Show).ToExpression(vm => vm.ActiveCategoryObjectItemView != null).OneWay();
            bindingSet.Bind(this.m_Title).For(v => v.text).To(vm => vm.CategoryObjectItemName).OneWay();
        }

        public void Init()
        {
            var allItems = ViewModel.AllItems;
            foreach (var buildableGen in BuildableAssets.Instance.buildableGenAssets.allAssets)
            {
                var categorySO = buildableGen.buildableObjectSO.buildableObjectUICategorySO;
                if (!ViewModel.AllItems.ContainsKey(categorySO))
                {
                    ViewModel.AllItems[categorySO] = new ObservableList<BuildableGenItemViewModel>();
                }
                var item = ViewModel.CreateItem(buildableGen);
                allItems[categorySO].Add(item);
            }
        }

        //private void OnActiveBuildableSOChanged(EasyGridBuilderPro easyGridBuilderPro, BuildableObjectSO buildableObjectSO)
        //{
        //    if (!ViewModel.temp_Lock && buildableObjectSO == null && ViewModel.SelectedItem != null)
        //    {
        //        ViewModel.SetSelectedItem(null);
        //    }
        //}

        private void OnActiveGridModeChanged(EasyGridBuilderPro easyGridBuilderPro, GridMode gridMode)
        {
            if (gridMode != GridMode.BuildMode &&  ViewModel.SelectedItem != null)
            {
                ViewModel.SetSelectedItem(null);
            }
        }
    }
}
