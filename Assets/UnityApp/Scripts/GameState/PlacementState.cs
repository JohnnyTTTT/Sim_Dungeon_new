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

        public PlacementState(GameStateManager manager)
        {
            this.manager = manager;
            m_MainGameViewModel = BindingService.MainGameViewModel;
            m_GridManager = GridManager.Instance;
        }

        public void Enter()
        {
            m_GridManager.SetActiveGridModeInAllGrids(GridMode.None);
            m_MainGameViewModel.GridType = GridType.Nothing;
            BindingService.CategoryObjectsPanelViewModel.ActiveBuildCategory = BuildCategory.Placement;

        }

        public void Exit()
        {
            BindingService.BuildableObjectsPanelViewModel.SetSelectedItem(null);
            BindingService.CategoryObjectsPanelViewModel.SetSelectedItem(null);
            BindingService.CategoryObjectsPanelViewModel.ActiveBuildCategory = BuildCategory.None;
        }

        public void Update()
        {
            /* 默认模式下的逻辑 */
        }
    }
}
