using Loxodon.Framework.Contexts;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Johnny.SimDungeon
{
    public class SelectionManager : MonoBehaviour
    {
        private const string DEFAULT = "Default";
        private const string FLOOD_FILL = "Flood Fill";
        private const string SELECT = "Select";

        public event Action<Entity> OnEntityHover;
        public event Action<Entity> OnEntitySelected;
        public event Action<Element_SmallCell> OnSmallCellHover;
        public event Action<Element_LargeCell> OnLargeCellHover;

        public static SelectionManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<SelectionManager>();
                }
                return s_Instance;
            }

        }
        private static SelectionManager s_Instance;

        [SerializeField] private LayerMask m_MouseCheckLayer;

        [SerializeField] private InputActionAsset m_InputActionsAsset;
        public InputAction floodFillAction;
        public InputAction selectAction;


        private bool m_AllowSelection;
        private GridManager m_GridManager;

        public Element_LargeCell HoverLargeCell
        {
            get
            {
                return m_HoverLargeCell;
            }
            set
            {
                m_HoverLargeCell = value;
                OnLargeCellHover?.Invoke(m_HoverLargeCell);
            }
        }
        private Element_LargeCell m_HoverLargeCell;
        private Vector2Int m_HoverLargeCoord = new Vector2Int(-999, -999);

        public Element_SmallCell HoverSmallCell
        {
            get
            {
                return m_HoverSmallCell;
            }
            set
            {
                m_HoverSmallCell = value;
                OnSmallCellHover?.Invoke(m_HoverSmallCell);
            }
        }
        private Element_SmallCell m_HoverSmallCell;
        private Vector2Int m_HoverSmallCoord = new Vector2Int(-999, -999);


        public Entity HoverEntity
        {
            get
            {
                return m_HoverEntity;
            }
            set
            {
                m_HoverEntity = value;
                OnEntityHover?.Invoke(m_HoverEntity);
            }
        }
        private Entity m_HoverEntity;

        public Entity SelectEntity
        {
            get
            {
                return m_SelectEntity;
            }
            set
            {
                m_SelectEntity = value;
                OnEntitySelected?.Invoke(m_SelectEntity);
            }
        }
        private Entity m_SelectEntity;

        private void Start()
        {
            var serviceContainer = Context.GetApplicationContext().GetContainer();

            m_GridManager = GridManager.Instance;

            var map = m_InputActionsAsset.FindActionMap(DEFAULT);

            floodFillAction = map.FindAction(FLOOD_FILL);
            floodFillAction.Enable();

            selectAction = map.FindAction(SELECT);
            selectAction.Enable();

            //selectAction.performed += OnSelectPerformed;

            m_GridManager.OnActiveGridModeChanged += OnActiveGridModeChanged;
            m_GridManager.OnActiveBuildableSOChanged += OnActiveBuildableSOChanged;

            if (m_GridManager.TryGetBuildableObjectDestroyer(out var buildableObjectDestroyer))
            {
                buildableObjectDestroyer.OnBuildableObjectDestroyedInternal += OnBuildableObjectDestroyedInternal;
            }
            if (m_GridManager.TryGetBuildableObjectMover(out var buildableObjectMover))
            {
                buildableObjectMover.OnBuildableObjectEndMoving += OnBuildableObjectEndMovingByBuildableObjectMoverDelegate;
            }

        }

        private void OnBuildableObjectEndMovingByBuildableObjectMoverDelegate(BuildableObject buildableObject)
        {
            Debug.Log(1);
        }

        private void OnActiveBuildableSOChanged(EasyGridBuilderPro easyGridBuilderPro, BuildableObjectSO buildableObjectSO)
        {

        }

        private void OnActiveGridModeChanged(EasyGridBuilderPro easyGridBuilderPro, GridMode gridMode)
        {
            SelectEntity = null;
            m_AllowSelection = gridMode == GridMode.None;
        }

        private void Update()
        {
            if (m_AllowSelection)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    HandleHover();
                    HandleClick();
                    HandleCancel();
                }
            }
        }

        private void OnBuildableObjectDestroyedInternal(EasyGridBuilderPro easyGridBuilderPro, BuildableObject buildableObject)
        {
            if (SelectEntity != null && SelectEntity.buildableObject == buildableObject)
            {
                SelectEntity = null;
            }
        }

        private void HandleHover()
        {
            if (PhysicsUtility.MouseRaycastHit(m_MouseCheckLayer, out var hit))
            {
                var point = hit.point;
                point.y = 0;

                //HoverSmallCell
                var smallCoord = CoordUtility.WorldPositionToSmallCoord(point);
                if (m_HoverSmallCoord != smallCoord)
                {
                    if (CoordUtility.IsSamllCoordInBounds(smallCoord))
                    {
                        HoverSmallCell = ElementManager_SmallCell.Instance.GetElement(smallCoord);
                    }
                    else
                    {
                        HoverSmallCell = null;
                    }
                    m_HoverSmallCoord = smallCoord;
                }

                //HoverLargeCell
                var largeCoord = CoordUtility.WorldPositionToLargeCoord(point);
                if (m_HoverLargeCoord != largeCoord)
                {
                    if (CoordUtility.IsLargeCoordInBounds(largeCoord))
                    {
                        HoverLargeCell = ElementManager_LargeCell.Instance.GetElement(point);
                    }
                    else
                    {
                        HoverLargeCell = null;
                    }
                    m_HoverLargeCoord = largeCoord;
                }

                //HoverEntity
                if (hit.transform.TryGetComponent<Entity>(out var entity))
                {
                    if (HoverEntity != entity)
                    {
                        // 移除旧 hover 高亮（如果不是选中）
                        if (HoverEntity != null && HoverEntity != SelectEntity)
                        {
                            HoverEntity = null;
                        }

                        HoverEntity = entity;
                    }
                    return;
                }

            }

            // 鼠标没指向任何对象 → 取消 hover（但不影响选中）
            if (HoverEntity != null && HoverEntity != SelectEntity)
            {
                HoverEntity = null;
            }
        }

        private void HandleClick()
        {
            if (Mouse.current == null) return;
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // 如果有旧的选中对象，取消高亮
                if (SelectEntity != null && SelectEntity != HoverEntity)
                {
                    SelectEntity = null;
                }

                // 设置新的选中对象并保持高亮
                if (HoverEntity != null && HoverEntity.canSelect)
                {
                    SelectEntity = HoverEntity;
                }
            }
        }

        private void HandleCancel()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                SelectEntity = null;
            }
        }
    }
}
