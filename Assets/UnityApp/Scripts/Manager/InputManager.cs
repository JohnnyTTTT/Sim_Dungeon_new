using SoulGames.EasyGridBuilderPro;
using System;
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

        public InputAction floodFillAction;
        public InputAction selectAction;


        private bool isPlacing;
        private GridManager m_GridManager;
        private void Start()
        {
            m_SelectionViewModel = BindingService.SelectionViewModel;
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
            if (m_SelectionViewModel.SelectEntity != null && m_SelectionViewModel.SelectEntity.buildableObject == buildableObject)
            {
                m_SelectionViewModel.SelectEntity = null;
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
                    m_SelectionViewModel.HoverSmallCell = ElementManager_SmallCell.Instance.GetElement(coord);
                }
                else
                {
                    m_SelectionViewModel.HoverSmallCell = null;
                }
                m_SelectionViewModel.HoverLargeCell = ElementManager_LargeCell.Instance.GetElement(point);

                if (hit.transform.TryGetComponent<Entity>(out var entity))
                {
                    if (m_SelectionViewModel.HoverEntity != entity)
                    {
                        // 移除旧 hover 高亮（如果不是选中）
                        if (m_SelectionViewModel.HoverEntity != null && m_SelectionViewModel.HoverEntity != m_SelectionViewModel.SelectEntity)
                            m_SelectionViewModel.HoverEntity.ShowOutline(false);

                        // 给新 hover 高亮（但不要覆盖选中的高亮）
                        if (entity != m_SelectionViewModel.SelectEntity)
                            entity.ShowOutline(true);

                        m_SelectionViewModel.HoverEntity = entity;
                    }
                    return;
                }

            }

            // 鼠标没指向任何对象 → 取消 hover（但不影响选中）
            if (m_SelectionViewModel.HoverEntity != null && m_SelectionViewModel.HoverEntity != m_SelectionViewModel.SelectEntity)
            {
                m_SelectionViewModel.HoverEntity.ShowOutline(false);
                m_SelectionViewModel.HoverEntity = null;
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
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                // 如果有旧的选中对象，取消高亮
                if (m_SelectionViewModel.SelectEntity != null && m_SelectionViewModel.SelectEntity != m_SelectionViewModel.HoverEntity)
                {
                    ClearSelection();
                }

                // 设置新的选中对象并保持高亮
                if (m_SelectionViewModel.HoverEntity != null && m_SelectionViewModel.HoverEntity.canSelect)
                {
                    m_SelectionViewModel.SelectEntity = m_SelectionViewModel.HoverEntity;
                    m_SelectionViewModel.SelectEntity.ShowOutline(true);
                    ShowRegionsRange(m_SelectionViewModel.SelectEntity, true);
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
            if (m_SelectionViewModel.SelectEntity != null)
            {
                m_SelectionViewModel.SelectEntity.ShowOutline(false);
                ShowRegionsRange(m_SelectionViewModel.SelectEntity, false);
                m_SelectionViewModel.SelectEntity = null;
            }
        }
    }
}
