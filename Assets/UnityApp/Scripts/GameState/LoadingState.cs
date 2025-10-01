using SoulGames.EasyGridBuilderPro;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class LoadingState : IGameState
    {
        public GameState StateID => GameState.Loading;
        private GameStateManager manager;

        public LoadingState(GameStateManager manager)
        {
            this.manager = manager;
        }

        public void Enter()
        {
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
