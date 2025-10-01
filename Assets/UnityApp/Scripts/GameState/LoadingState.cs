using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class LoadingState : IGameState
    {
        public GameState StateID => GameState.Loading;
        private GameStateManager manager;
        private GridManager m_GridManager;

    

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
        }
    }
}
