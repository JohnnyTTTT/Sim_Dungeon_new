using Loxodon.Framework.Messaging;
using SoulGames.EasyGridBuilderPro;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public interface IBuildGameState
    {
        DestroyMode DestroyMode { get; set; }

        void ChangeDestroyMode();
    }

    public class StructureState : IGameState
    {
        public GameState StateID => GameState.Structure;




        private GameStateManager manager;
        private GridManager m_GridManager;
        public DestroyMode destroyMode;

        public StructureState(GameStateManager manager)
        {
            this.manager = manager;
            //m_MainGameViewModel = BindingService.MainGameViewModel;
            //m_GridManager = GridManager.Instance;
        }

        public void Enter()
        {
            //m_GridManager.SetActiveGridModeInAllGrids(GridMode.None);
            //m_MainGameViewModel.GridType = GridType.Nothing;
            //BindingService.BuildableObjectsPanelViewModel.ActiveCategoryObjectItemView = null;
            //BindingService.CategoryObjectsPanelViewModel.ActiveBuildCategory = BuildCategory.Structure;
        }

        public void Exit()
        {
            destroyMode = DestroyMode.None;
            //BindingService.BuildableObjectsPanelViewModel.ActiveCategoryObjectItemView = null;
            //BindingService.CategoryObjectsPanelViewModel.ActiveBuildCategory = BuildCategory.None;
        }

        public void Update()
        {
            /* 默认模式下的逻辑 */
        }

        public void ChangeDestroyMode()
        {
            throw new System.NotImplementedException();
        }
    }
}
