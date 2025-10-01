using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
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
                    Loxodon.Framework.Messaging.Messenger.Default.Publish(new PropertyChangedMessage<GameState>(m_GameState, value, nameof(GameState)));
                    Set(ref m_GameState, value);
                    GameStateManager.Instance.ChangeState(m_GameState);
                }
            }
        }
        private GameState m_GameState = GameState.Default;
    }


    public class MainGameView : MonoBehaviour
    {
        public static MainGameView Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<MainGameView>();
                }
                return s_Instance;
            }

        }
        private static MainGameView s_Instance;

        private GridManager m_GridManager;

        protected void Start()
        {
            m_GridManager = GridManager.Instance;
            m_GridManager.OnActiveEasyGridBuilderProChanged += OnActiveEasyGridBuilderProChanged;
            m_GridManager.OnActiveGridModeChanged += OnActiveGridModeChanged;
            m_GridManager.OnActiveBuildableSOChanged += OnActiveBuildableSOChanged;
        }


        private void OnActiveEasyGridBuilderProChanged(EasyGridBuilderPro activeEasyGridBuilderProSystem)
        {
            if (activeEasyGridBuilderProSystem != null)
            {
                Debug.Log($"<GridType Changed> - {activeEasyGridBuilderProSystem.name.SetColor(Color.softYellow)}");
            }
            else
            {
                Debug.Log($"<GridType Changed> - {"NULL".SetColor(Color.softYellow)}");
            }
        }

        private void OnActiveBuildableSOChanged(EasyGridBuilderPro easyGridBuilderPro, BuildableObjectSO buildableObjectSO)
        {
            if (buildableObjectSO != null)
            {
                Debug.Log($"<BuildableObjectSO Changed> - ,{easyGridBuilderPro.name} , { buildableObjectSO.objectName.SetColor(Color.blue)}");
            }
            else
            {
                Debug.Log($"<BuildableObjectSO Changed> - {"NULL".SetColor(Color.blue)}");
            }
        }

        private void OnActiveGridModeChanged(EasyGridBuilderPro easyGridBuilderPro, GridMode gridMode)
        {
            Debug.Log($"<GridMode Changed> - {easyGridBuilderPro.name} , {gridMode.SetColor(Color.yellow)}");
            //switch (gridMode)
            //{
            //    case GridMode.None:
            //        ViewModel.IsLandExpandMode = false;
            //        //ViewModel.IsDestroyMode = false;
            //        break;
            //    case GridMode.BuildMode:
            //        //ViewModel.IsDestroyMode = false;
            //        break;
            //    case GridMode.DestroyMode:
            //        ViewModel.IsLandExpandMode = false;
            //        break;
            //    case GridMode.SelectMode:
            //        ViewModel.IsLandExpandMode = false;
            //        //ViewModel.IsDestroyMode = false;
            //        break;
            //    case GridMode.MoveMode:
            //        ViewModel.IsLandExpandMode = false;
            //        break;
            //    default:
            //        break;
            //}
        }



    }
}
