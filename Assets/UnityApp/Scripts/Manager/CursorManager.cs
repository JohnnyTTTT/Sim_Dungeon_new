using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public enum CursorType
    {
        None = 0,
        Hand =10,
        Build =20,
        Detroy,
        Move,
    }

    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<CursorManager>();
                }
                return s_Instance;
            }
        }
        private static CursorManager s_Instance;
        public static CursorType CursorType
        {
            get
            {
                return s_CursorType;
            }
            set
            {
                if (s_CursorType != value)
                {
                    s_CursorType = value;
                    CursorManager.Instance.CursorTypeChanged(s_CursorType);
                }
            }
        }
        private static CursorType s_CursorType;

        [SerializeField] private CursorAssets m_CursorAssets;
        private Dictionary<CursorType, Texture2D> m_CursorAssetsDic = new Dictionary<CursorType, Texture2D>();

        private void Awake()
        {
            foreach (var item in m_CursorAssets.cursorAssets)
            {
                m_CursorAssetsDic.Add(item.cursorMode, item.texture);
            }
        }

        private void Start()
        {
            //DungeonController.Instance.OnWorldCreated += OnWorldCreated;
            GridManager.Instance.OnActiveGridModeChanged += OnActiveGridModeChanged;
        }

        private void OnActiveGridModeChanged(EasyGridBuilderPro easyGridBuilderPro, GridMode gridMode)
        {
            switch (gridMode)
            {
                case GridMode.None:
                    CursorType = CursorType.None;
                    break;
                case GridMode.BuildMode:
                    CursorType = CursorType.None;
                    break;
                case GridMode.DestroyMode:
                    CursorType = CursorType.Detroy;
                    break;
                case GridMode.SelectMode:
                    break;
                case GridMode.MoveMode:
                    break;
                default:
                    break;
            }
        }

        private void OnWorldCreated()
        {
            CursorType = CursorType.None;
        }

        private void CursorTypeChanged(CursorType s_CursorMode)
        {
            if (m_CursorAssetsDic.TryGetValue(s_CursorMode, out var texture))
            {
                Cursor.SetCursor(texture, Vector2.zero, UnityEngine.CursorMode.Auto);
            }
        }



    }
}
