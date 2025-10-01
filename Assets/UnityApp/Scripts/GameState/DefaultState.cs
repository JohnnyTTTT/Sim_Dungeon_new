using Loxodon.Framework.Contexts;
using SoulGames.EasyGridBuilderPro;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class DefaultState : IGameState
    {
        public GameState StateID => GameState.Default;
        private GameStateManager manager;
        private MainGameViewModel m_MainGameViewModel;
        private GridManager m_GridManager;

        public DefaultState(GameStateManager manager)
        {
            this.manager = manager;
            var serviceContainer = Context.GetApplicationContext().GetContainer();
            m_MainGameViewModel = serviceContainer.Resolve<MainGameViewModel>();
            m_GridManager = GridManager.Instance;
        }

        public void Enter()
        {
            m_GridManager.SetActiveGridModeInAllGrids(GridMode.None);
            SpawnManager.Instance.SetGridType(GridType.Nothing);
        }

        public void Exit()
        {
        }

        public void Update()
        {
            /* 默认模式下的逻辑 */
        }
    }
}
