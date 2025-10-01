using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using SoulGames.EasyGridBuilderPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Profiling.HierarchyFrameDataView;

namespace Johnny.SimDungeon
{
    public class FixedToolbarViewModel : ListViewModel<SelectableItemViewModel>
    {

        public DestroyToolViewModel CreateDestroyToolViewModel()
        {
            var item = new DestroyToolViewModel(this.ItemSelectCommand);
            Items.Add(item);
            return item;
        }
    }

    public class FixedToolbarView : ViewBase<FixedToolbarViewModel>
    {
        [SerializeField] private DestroyToolItemView m_DestroyButton;
        [SerializeField] private AnimationPanel m_AnimationPanel;

        protected override void Start()
        {
            //ViewModel = BindingService.FixedToolbarViewModel;
            //m_DestroyButton.ViewModel = ViewModel.CreateDestroyToolViewModel();
            //GridManager.Instance.OnActiveGridModeChanged += OnActiveGridModeChanged;
            base.Start();
        }

        protected override void Binding(BindingSet<ViewBase<FixedToolbarViewModel>, FixedToolbarViewModel> bindingSet)
        {

        }

        protected override void StaticBinding(BindingSet<ViewBase<FixedToolbarViewModel>> staticBindingSet)
        {
            //staticBindingSet.Bind(this.m_AnimationPanel).For(v => v.Show).ToExpression(() =>
            //BindingService.MainGameViewModel.GameState == GameState.Structure ||
            //BindingService.MainGameViewModel.GameState == GameState.Placement)
            //    .OneWay();
        }

        private void OnActiveGridModeChanged(EasyGridBuilderPro easyGridBuilderPro, GridMode gridMode)
        {

        }
    }
}
