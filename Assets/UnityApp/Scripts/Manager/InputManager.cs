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
    public class InputManager : MonoBehaviour
    {
        private const string DEFAULT = "Default";
        private const string FLOOD_FILL = "Flood Fill";
        private const string SELECT = "Select";

        public event Action<Entity> OnEntitySelect;

        public static InputManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<InputManager>();
                }
                return s_Instance;
            }

        }
        private static InputManager s_Instance;

        [SerializeField] private LayerMask m_MouseCheckLayer;
        [SerializeField] private InputActionAsset m_InputActionsAsset;

        private SelectionViewModel m_SelectionViewModel;
        private CellInfoViewModel m_CellInfoViewModel;

        public InputAction floodFillAction;
        public InputAction selectAction;


        private bool isPlacing;
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
                m_CellInfoViewModel.HoverLargeCell = m_HoverLargeCell;
            }
        }
        private Element_LargeCell m_HoverLargeCell;

        public Element_SmallCell HoverSmallCell
        {
            get
            {
                return m_HoverSmallCell;
            }
            set
            {
                m_HoverSmallCell = value;
                m_CellInfoViewModel.HoverSmallCell = m_HoverSmallCell = value;
            }
        }
        private Element_SmallCell m_HoverSmallCell;

        public Entity HoverEntity
        {
            get
            {
                return m_HoverEntity;
            }
            set
            {
                m_HoverEntity = value;
                m_SelectionViewModel.HoverEntity = m_HoverEntity;
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
                m_SelectionViewModel.SelectEntity = m_SelectEntity;
            }
        }
        private Entity m_SelectEntity;

        private void Start()
        {
            var serviceContainer = Context.GetApplicationContext().GetContainer();
            m_SelectionViewModel = serviceContainer.Resolve<SelectionViewModel>();
            m_CellInfoViewModel = serviceContainer.Resolve<CellInfoViewModel>();

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
            ClearSelection();
            isPlacing = gridMode == GridMode.BuildMode;
        }

        private void Update()
        {

            if (isPlacing)
            {

            }
            else
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

                var coord = CoordUtility.WorldPositionToSmallCoord(point);
                if (SpawnManager.Instance.IsSamllCoordInBounds(coord))
                {
                    HoverSmallCell = ElementManager_SmallCell.Instance.GetElement(coord);
                }
                else
                {
                    HoverSmallCell = null;
                }
                HoverLargeCell = ElementManager_LargeCell.Instance.GetElement(point);

                if (hit.transform.TryGetComponent<Entity>(out var entity))
                {
                    if (HoverEntity != entity)
                    {
                        // 移除旧 hover 高亮（如果不是选中）
                        if (HoverEntity != null && HoverEntity != SelectEntity)
                            HoverEntity.ShowOutline(false);

                        // 给新 hover 高亮（但不要覆盖选中的高亮）
                        if (entity != SelectEntity)
                            entity.ShowOutline(true);

                        HoverEntity = entity;
                    }
                    return;
                }

            }

            // 鼠标没指向任何对象 → 取消 hover（但不影响选中）
            if (HoverEntity != null && HoverEntity != SelectEntity)
            {
                HoverEntity.ShowOutline(false);
                HoverEntity = null;
            }
        }

        private void ShowRegionsRange(Entity entity, bool value)
        {
            var buildableObject = entity.buildableObject;
            var positionList = buildableObject.GetObjectCellPositionList();

            foreach (var position in positionList)
            {
                ElementManager_Region.Instance.ShowRegionRangeFromSmallCoord(position, value);
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
                    ClearSelection();
                }

                // 设置新的选中对象并保持高亮
                if (HoverEntity != null && HoverEntity.canSelect)
                {
                    SelectEntity = HoverEntity;
                    SelectEntity.ShowOutline(true);
                    ShowRegionsRange(SelectEntity, true);
                }
            }
        }

        private void HandleCancel()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                ClearSelection();
            }
        }

        public void ClearSelection()
        {
            if (SelectEntity != null)
            {
                SelectEntity.ShowOutline(false);
                ShowRegionsRange(SelectEntity, false);
                SelectEntity = null;
            }
        }
    }
}
