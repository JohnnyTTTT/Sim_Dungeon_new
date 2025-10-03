using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public enum GameState
    {
        None = 0,
        Loading,
        God = 10,
        Default = 20,
        Structure,
        Placement,
    }

    public enum DestroyMode
    {
        None,
        Entity,
        Edge,
    }

    public class MainGameViewModel : ViewModelBase
    {
        public GameState GameState
        {
            get
            {
                return m_GameState;
            }
            set
            {
                if (m_GameState != value)
                {
                    Loxodon.Framework.Messaging.Messenger.Default.Publish(new PropertyChangedMessage<GameState>(m_GameState, value, nameof(this.GameState)));
                    Set(ref m_GameState, value);
                }
            }
        }
        private GameState m_GameState = GameState.Default;

        public GridMode GridMode
        {
            get
            {
                return m_GridMode;
            }
             set
            {
                Set(ref m_GridMode, value);
            }
        }
        private GridMode m_GridMode;

    }


    public class MainGameView : UIView
    {
        [SerializeField] private CategoryObjectsPanelView m_CategoryObjectsPanelView;
        //[SerializeField] private FixedToolbarView m_FixedToolbarView;

        private MainGameViewModel m_ViewModel;
        //private BuildableObjectsPanelViewModel m_BuildableObjectsPanelViewModel;

        protected override void Awake()
        {
            m_ViewModel = new MainGameViewModel();
            this.SetDataContext(m_ViewModel);

            var serviceContainer = Context.GetApplicationContext().GetContainer();
            serviceContainer.Register(m_ViewModel);
        }

        protected override void Start()
        {
            //var serviceContainer = Context.GetApplicationContext().GetContainer();
            //m_BuildableObjectsPanelViewModel = serviceContainer.Resolve<BuildableObjectsPanelViewModel>();

            var bindingSet = this.CreateBindingSet<MainGameView, MainGameViewModel>();

            //bindingSet.Bind(this.m_CategoryObjectsPanelView).For(v => v.Visibility)
            //    .ToExpression(vm => (vm.GameState == GameState.Structure || vm.GameState == GameState.Placement)
            //    && vm.GridMode != GridMode.DestroyMode)
            //    .OneWay();

            //bindingSet.Bind(this.m_FixedToolbarView).For(v => v.Visibility)
            //    .ToExpression(vm => vm.GameState == GameState.Structure || vm.GameState == GameState.Placement)
            //    .OneWay();

            bindingSet.Build();
        }
    }
}
