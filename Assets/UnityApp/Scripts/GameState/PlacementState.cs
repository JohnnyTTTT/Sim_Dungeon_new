using Loxodon.Framework.Contexts;
using Loxodon.Framework.Services;
using SoulGames.EasyGridBuilderPro;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class PlacementState : IGameState
    {
        public GameState StateID => GameState.Placement;
        private GameStateManager manager;
        private MainGameViewModel m_MainGameViewModel;
        private GridManager m_GridManager;
        private IServiceContainer m_ServiceContainer;

        public PlacementState(GameStateManager manager)
        {
            this.manager = manager;
            //m_MainGameViewModel = BindingService.MainGameViewModel;
            //m_GridManager = GridManager.Instance;
            //m_ServiceContainer = Context.GetApplicationContext().GetContainer();
        }

        public void Enter()
        {
            //m_GridManager.SetActiveGridModeInAllGrids(GridMode.None);
            //m_MainGameViewModel.GridType = GridType.Nothing;
            //BindingService.BuildableObjectsPanelViewModel.ActiveCategoryObjectItemView = null;
            //var vm = m_ServiceContainer.Resolve<CategoryObjectsPanelViewModel>();
            //vm.ActiveBuildCategory = BuildCategory.Placement;
            //BindingService.CategoryObjectsPanelViewModel.ActiveBuildCategory = BuildCategory.Placement;

        }

        public void Exit()
        {
            //var vm = m_ServiceContainer.Resolve<CategoryObjectsPanelViewModel>();
            //vm.ActiveBuildCategory = BuildCategory.None;
            //BindingService.BuildableObjectsPanelViewModel.ActiveCategoryObjectItemView = null;
            //BindingService.CategoryObjectsPanelViewModel.ActiveBuildCategory = BuildCategory.None;
        }

        public void Update()
        {
            /* 默认模式下的逻辑 */
        }
    }
}
