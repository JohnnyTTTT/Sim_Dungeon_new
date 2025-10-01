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
            m_MainGameViewModel = BindingService.MainGameViewModel;
            m_GridManager = GridManager.Instance;
        }

        public void Enter()
        {
            m_GridManager.SetActiveGridModeInAllGrids(GridMode.None);
            m_MainGameViewModel.GridType = GridType.Nothing;
        }

        public void Exit()
        {
        }

        public void Update()
        {
            /* Ĭ��ģʽ�µ��߼� */
        }
    }
}
