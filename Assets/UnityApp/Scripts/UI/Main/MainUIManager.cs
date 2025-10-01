using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Execution;
using Loxodon.Framework.ViewModels;
using Michsky.MUIP;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Johnny.SimDungeon
{


    public enum StructureMode
    {
        None,
        LandExpand,
    }


    public class MainUIManager : ViewBase<MainGameViewModel>
    {
        public static MainUIManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<MainUIManager>();
                }
                return s_Instance;
            }
        }
        private static MainUIManager s_Instance;

        [SerializeField] private CategoryObjectsPanelView m_CategoryObjectsPanelView;
        [SerializeField] private BuildableObjectsPanelView m_BuildableObjectsPanelView;



        private GridManager m_GridManager;
        private bool m_Inited;

        protected override void Start()
        {
            //ViewModel = BindingService.MainGameViewModel;
            m_GridManager = GridManager.Instance;
            base.Start();
        }


        protected override void Binding(BindingSet<ViewBase<MainGameViewModel>, MainGameViewModel> bindingSet)
        {
            //bindingSet.Bind(this.m_DestroyToggle).For(v => v.isOn, v => v.onValueChanged).To(vm => vm.IsDestroyMode).TwoWay();
            //bindingSet.Bind(this.m_LandExpandToggle).For(v => v.isOn, v => v.onValueChanged).To(vm => vm.IsLandExpandMode).TwoWay();
        }

    }
}
