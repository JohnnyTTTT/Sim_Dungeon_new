using Loxodon.Framework.Contexts;
using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Johnny.SimDungeon
{
    public class ZonePlacementGhost : MonoBehaviour
    {
        [ShowInInspector] [ReadOnly] private ZoneType m_CurrentZoneType;
        [ShowInInspector] [ReadOnly] private List<GameObject> m_CandidateRegionCells = new List<GameObject>();
        [ShowInInspector] [ReadOnly] private BuildableGridObjectSO m_CandidateBuildableGridObjectSO;
        [ShowInInspector] [ReadOnly] private bool m_IsGridBoxPlacemen;

        private Vector2Int m_LastDetectionCoord;
        private GridManager m_GridManager;
        private SelectionManager m_InputManager;
        private CellInfoViewModel m_SelectionViewModel;

        private void Start()
        {
            var serviceContainer = Context.GetApplicationContext().GetContainer();
            m_SelectionViewModel = serviceContainer.Resolve<CellInfoViewModel>();

            m_GridManager = GridManager.Instance;
            m_GridManager.OnActiveBuildableSOChanged += OnActiveBuildableSOChanged;
            m_GridManager.OnBuildableObjectPlaced += OnBuildableObjectPlaced;

            m_GridManager.OnGridObjectBoxPlacementStarted += OnGridObjectBoxPlacementStarted;
            m_GridManager.OnGridObjectBoxPlacementFinalized += OnGridObjectBoxPlacementFinalized;
            m_GridManager.OnGridObjectBoxPlacementCancelled += OnGridObjectBoxPlacementCancelled;

            m_InputManager = SelectionManager.Instance;
            m_InputManager.floodFillAction.performed += FloodFillActionPerformed;
            m_InputManager.floodFillAction.canceled += FloodFillActionCanceled;
        }

        private void Update()
        {
            if (!m_IsGridBoxPlacemen && m_CurrentZoneType != ZoneType.Undefined)
            {
                if (m_LastDetectionCoord != m_SelectionViewModel.HoverLargeCell.coord)
                {
                    m_LastDetectionCoord = m_SelectionViewModel.HoverLargeCell.coord;
                    if (m_CandidateRegionCells.Count > 0)
                    {
                        FloodFillActionCanceled(default);
                        FloodFillActionPerformed(default);
                    }
                }
            }
        }

        private void OnGridObjectBoxPlacementStarted(EasyGridBuilderPro easyGridBuilderPro, Vector3 boxPlacementStartPosition, GridObjectPlacementType placementType)
        {
            m_IsGridBoxPlacemen = true;
        }

        private void OnGridObjectBoxPlacementFinalized(EasyGridBuilderPro easyGridBuilderPro)
        {
            if (m_CurrentZoneType != ZoneType.Undefined && m_CandidateRegionCells.Count > 0)
            {
                foreach (var item in m_CandidateRegionCells)
                {
                    SpawnManager.Instance.TryInitializeBuildableGridObjectSinglePlacement(
                        SpawnManager.Instance.m_EasyGridBuilderPro_LargeCell,
                        item.transform.position, FourDirectionalRotation.North,
                        m_CandidateBuildableGridObjectSO, out _);
                }
                FloodFillActionCanceled(default);
            }
            m_IsGridBoxPlacemen = false;
        }

        private void OnGridObjectBoxPlacementCancelled(EasyGridBuilderPro easyGridBuilderPro)
        {
            FloodFillActionCanceled(default);
            m_IsGridBoxPlacemen = false;
        }

        private void OnActiveBuildableSOChanged(EasyGridBuilderPro easyGridBuilderPro, BuildableObjectSO buildableObjectSO)
        {
            if (m_CurrentZoneType == ZoneType.Undefined && buildableObjectSO is BuildableGridObjectSO buildableGridObjectSO)
            {
                if (buildableGridObjectSO.randomPrefabs[0].objectPrefab.TryGetComponent<Entity_ZoneCell>(out var entity_ZoneCell))
                {
                    m_CurrentZoneType = entity_ZoneCell.zoneType;
                    m_CandidateBuildableGridObjectSO = buildableGridObjectSO;
                }
            }
            else if (buildableObjectSO == null)
            {
                m_CurrentZoneType = ZoneType.Undefined;
                Clear();
            }
        }

        private void OnBuildableObjectPlaced(EasyGridBuilderPro easyGridBuilderPro, BuildableObject buildableObject)
        {
            //if (buildableObject is BuildableEdgeObject buildable)
            //{
            //    foreach (var item in buildable.GetCellPositionDictionary())
            //    {
            //        Debug.Log(item.Key+" "+ item.Value);
            //    }
            //}

        }

        private void FloodFillActionPerformed(InputAction.CallbackContext obj)
        {
            if (!m_IsGridBoxPlacemen && m_CurrentZoneType != ZoneType.Undefined && m_LastDetectionCoord != null)
            {
                var region =    ElementManager_Region.Instance.GetRegionFromLargeCoord(m_LastDetectionCoord);
                var prefab = m_CandidateBuildableGridObjectSO.randomPrefabs[0].ghostObjectPrefab;
                foreach (var regionCell in region.containedLargeCells)
                {
                    if (regionCell.coord != m_LastDetectionCoord)
                    {
                        var ghost = Instantiate(prefab, regionCell.worldPosition, Quaternion.identity, this.transform).gameObject;
                        m_CandidateRegionCells.Add(ghost);
                    }
                }
            }
        }

        private void FloodFillActionCanceled(InputAction.CallbackContext obj)
        {
            for (int i = m_CandidateRegionCells.Count - 1; i >= 0; i--)
            {
                Destroy(m_CandidateRegionCells[i]);
            }
            m_CandidateRegionCells.Clear();
        }

        private void Clear()
        {
            m_CurrentZoneType = ZoneType.Undefined;
            for (int i = m_CandidateRegionCells.Count - 1; i >= 0; i--)
            {
                Destroy(m_CandidateRegionCells[i]);
            }
            m_CandidateRegionCells.Clear();
            m_CandidateBuildableGridObjectSO = null;
        }
    }
}
