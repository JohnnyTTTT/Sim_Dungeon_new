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

        // �������ԣ����ڲ�ѯ��ǰģʽ
        public GameState CurrentMode => currentState.StateID;

        /// <summary>
        /// ʵ������ע��������Ϸģʽ״̬
        /// </summary>
        public void Initialize()
        {
            allStates = new Dictionary<GameState, IGameState>();

            // ʵ�������о���״̬����ӵ��ֵ���
            RegisterState(new LoadingState(this));
            RegisterState(new DefaultState(this));
            RegisterState(new StructureState(this));
            RegisterState(new PlacementState(this));

            // ��ʼ״̬����Ϊ None
            currentState = allStates[GameState.Loading];
            currentState.Enter();
        }

        private void RegisterState(IGameState state)
        {
            allStates.Add(state.StateID, state);
        }

        /// <summary>
        /// ���ķ�����״̬ת��
        /// </summary>
        /// <param name="newMode">Ŀ��״̬��ʶ��</param>
        public void ChangeMode(GameState newMode)
        {
            if (newMode == currentState.StateID)
                return; // Ŀ��״̬�Ѿ��ǵ�ǰ״̬

            if (allStates.TryGetValue(newMode, out IGameState newState))
            {
                // 1. �˳���ǰ״̬
                currentState.Exit();

                // 2. �л�����״̬
                currentState = newState;

                // 3. ������״̬
                currentState.Enter();

                // ��ѡ���ڴ˴������¼�֪ͨ�ⲿģ�飨���� UI ˢ�£�
                // OnGameModeChanged?.Invoke(newMode); 

                Debug.Log($"<GameMode Switched> - {currentState.StateID.SetColor(Color.yellowGreen)}");
            }
            else
            {
                Debug.Log($"<GameMode Switched> - {"not found!".SetColor(Color.yellowGreen)}");
            }
        }

        /// <summary>
        /// ����ѭ���е��ã���ִ�е�ǰ״̬�� Update �߼�
        /// </summary>
        public void UpdateCurrentState()
        {
            currentState.Update();
        }
    }
}