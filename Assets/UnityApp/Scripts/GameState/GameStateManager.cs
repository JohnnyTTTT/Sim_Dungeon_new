using DungeonArchitect.UI;
using Johnny.SimDungeon;
using System;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;
namespace Johnny.SimDungeon
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<GameStateManager>();
                }
                return s_Instance;
            }

        }
        private static GameStateManager s_Instance;

        private Dictionary<GameState, IGameState> allStates;
        private IGameState currentState;
        private MainGameViewModel m_MainGameViewModel;

        // 公开属性，用于查询当前模式
        public GameState CurrentMode => currentState.StateID;

        /// <summary>
        /// 实例化并注册所有游戏模式状态
        /// </summary>
        public void Initialize()
        {
            allStates = new Dictionary<GameState, IGameState>();

            // 实例化所有具体状态并添加到字典中
            RegisterState(new LoadingState(this));
            RegisterState(new DefaultState(this));
            RegisterState(new StructureState(this));
            RegisterState(new PlacementState(this));

            // 初始状态设置为 None
            currentState = allStates[GameState.Loading];
            currentState.Enter();
        }

        private void RegisterState(IGameState state)
        {
            allStates.Add(state.StateID, state);
        }

        /// <summary>
        /// 核心方法：状态转换
        /// </summary>
        /// <param name="newMode">目标状态标识符</param>
        public void ChangeMode(GameState newMode)
        {
            if (newMode == currentState.StateID)
                return; // 目标状态已经是当前状态

            if (allStates.TryGetValue(newMode, out IGameState newState))
            {
                // 1. 退出当前状态
                currentState.Exit();

                // 2. 切换到新状态
                currentState = newState;

                // 3. 进入新状态
                currentState.Enter();

                // 可选：在此处触发事件通知外部模块（例如 UI 刷新）
                // OnGameModeChanged?.Invoke(newMode); 

                Debug.Log($"<GameMode Switched> - {currentState.StateID.SetColor(Color.yellowGreen)}");
            }
            else
            {
                Debug.Log($"<GameMode Switched> - {"not found!".SetColor(Color.yellowGreen)}");
            }
        }

        /// <summary>
        /// 在主循环中调用，以执行当前状态的 Update 逻辑
        /// </summary>
        public void UpdateCurrentState()
        {
            currentState.Update();
        }
    }
}