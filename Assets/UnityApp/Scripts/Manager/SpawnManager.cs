using DungeonArchitect;
using DungeonArchitect.Flow.Domains.Tilemap;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Messaging;
using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

namespace Johnny.SimDungeon
{
    public enum StructureMode
    {
        None,
        LandExpand,
    }

    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<SpawnManager>();
                }
                return s_Instance;
            }

        }
        private static SpawnManager s_Instance;

        [Title("Global Settings")]
        public Transform m_SpawnRoot;
        public Shader WallShader;
        public List<Entity> spwanedEntityForEditor = new List<Entity>();

        [Title("Easy GridBuilder Pro Settings")]

        [ShowInInspector]
        public GridType GridType
        {
            get
            {
                return m_GridType;
            }
            private set
            {
                m_GridType = value;
            }
        }
        private GridType m_GridType;

        public DestroyMode DestroyMode
        {
            get
            {
                return m_DestroyMode;
            }
            private set
            {
                m_DestroyMode = value;
            }
        }
        private DestroyMode m_DestroyMode;

        public GridMode GridMode
        {
            get
            {
                return m_GridMode;
            }
            private set
            {
                m_GridMode = value;
                m_MainGameViewModel.GridMode = m_GridMode;
            }
        }
        private GridMode m_GridMode;

        public EasyGridBuilderProXZ m_EasyGridBuilderPro_SmallCell;
        public EasyGridBuilderProXZ m_EasyGridBuilderPro_LargeCell;

        [Title("Default BuildableGridObjectSO")]
        public BuildableCornerObjectSO defaultFloor;
        public BuildableEdgeObjectSO defaultWall;
        public BuildableCornerObjectSO defaultPillar;
        public BuildableFreeObjectSO defaultDoor;


        private GridManager m_GridManager;
        private BuildableObjectDestroyer m_BuildableObjectDestroyer;
        private BuildableObjectSelector m_BuildableObjectSelector;
        private BuildableObjectMover m_BuildableObjectMover;
        private BuildableObjectsPanelViewModel m_BuildableObjectsPanelViewModel;
        private MainGameViewModel m_MainGameViewModel;

        [SerializeField] private List<BuildableGridObject> m_CandidateAreaExpandProxies = new List<BuildableGridObject>();
        [SerializeField] private List<Entity_Wall> m_CreatedBuildableEdgeObject = new List<Entity_Wall>();


        private void Start()
        {
            var serviceContainer = Context.GetApplicationContext().GetContainer();
            m_MainGameViewModel = serviceContainer.Resolve<MainGameViewModel>();
            m_BuildableObjectsPanelViewModel = serviceContainer.Resolve<BuildableObjectsPanelViewModel>();

            m_GridManager = GridManager.Instance;
            //m_GridManager.OnActiveGridModeChanged += OnActiveGridModeChanged;
            //m_GridManager.OnActiveBuildableSOChanged += OnActiveBuildableSOChanged;
            m_GridManager.OnBuildableObjectPlaced += OnBuildableObjectPlaced;
            m_GridManager.OnGridObjectBoxPlacementFinalized += OnGridObjectBoxPlacementFinalized;
            m_GridManager.OnEdgeObjectBoxPlacementFinalized += OnGridObjectBoxPlacementFinalized;
            m_GridManager.OnEdgeObjectBoxPlacementFinalized += OnEdgeObjectBoxPlacementFinalized;
            if (m_GridManager.TryGetBuildableObjectDestroyer(out var buildableObjectDestroyer))
            {
                m_BuildableObjectDestroyer = buildableObjectDestroyer;
            }
            if (GridManager.Instance.TryGetBuildableObjectMover(out var buildableObjectMover))
            {
                m_BuildableObjectMover = buildableObjectMover;
            }
            if (GridManager.Instance.TryGetBuildableObjectSelector(out var buildableObjectSelector))
            {
                m_BuildableObjectSelector = buildableObjectSelector;
            }
            //m_GridManager.GetActiveEasyGridBuilderPro().GetActiveGridCellData
        }

        public void Init()
        {
            if (Application.isPlaying)
            {
                if (m_GridManager.TryGetGridBuiltObjectsManager(out var gridBuiltObjectsManager))
                {
                    var objs = gridBuiltObjectsManager.GetBuiltObjectsList();
                    foreach (var item in objs)
                    {
                        if (item.TryGetComponent<Entity>(out var entity))
                        {
                            //if (item is Entity_Door door)
                            //{
                            //    doors.Add(door);
                            //}
                            entity.UpdateData();
                        }
                    }
                }
            }
            else
            {
                foreach (var entity in spwanedEntityForEditor)
                {
                    entity.UpdateData();
                }
                spwanedEntityForEditor.Clear();
            }



            //foreach (var item in doors)
            //{
            //    item.CutWall();
            //}

        }

        public void SetInputActiveBuildableObjectSO(BuildableObjectSO buildableObjectSO, GridType gridType)
        {
            if (GridMode != GridMode.BuildMode)
            {
                SetActiveGridModeInAllGrids(GridMode.BuildMode);
            }

            SetGridType(gridType);

            if (buildableObjectSO != null)
            {
                foreach (var easyGridBuilderPro in m_GridManager.GetEasyGridBuilderProSystemsList())
                {
                    easyGridBuilderPro.SetInputActiveBuildableObjectSO(buildableObjectSO, null, true);
                }
            }
        }

        private void OnActiveBuildableSOChanged(EasyGridBuilderPro easyGridBuilderPro, BuildableObjectSO buildableObjectSO)
        {

        }

        public void GridModeReset()
        {
            if (m_BuildableObjectsPanelViewModel.SelectedItem != null)
            {
                m_BuildableObjectsPanelViewModel.SetSelectedItem(null);
            }
            foreach (var easyGridBuilderPro in m_GridManager.GetEasyGridBuilderProSystemsList())
            {
                easyGridBuilderPro.SetInputGridModeReset();
            }
            m_BuildableObjectMover.SetInputGridModeReset();
            m_BuildableObjectDestroyer.SetInputGridModeReset();
            m_BuildableObjectSelector.SetInputGridModeReset();
            SetGridType(GridType.Nothing);
        }

        public void SetActiveGridModeInAllGrids(GridMode gridMode)
        {
            m_GridManager.SetActiveGridModeInAllGrids(gridMode, false);
        }

        public void SetDestroyModeInAllGrids(DestroyMode mode)
        {
            if (DestroyMode != mode)
            {
                switch (mode)
                {
                    case DestroyMode.None:
                        SetActiveGridModeInAllGrids(GridMode.None);
                        break;
                    case DestroyMode.Edge:
                        m_BuildableObjectDestroyer.SetInputDestructableBuildableObjectType(DestructableBuildableObjectType.BuildableEdgeObject);
                        SetActiveGridModeInAllGrids(GridMode.DestroyMode);
                        break;
                    case DestroyMode.Entity:
                        m_BuildableObjectDestroyer.SetInputDestructableBuildableObjectType(DestructableBuildableObjectType.BuildableGridObject);
                        SetActiveGridModeInAllGrids(GridMode.DestroyMode);
                        break;
                    default:
                        break;
                }
                DestroyMode = mode;
            }

        }

        public void SetInputBuildableObjectClockwiseRotation(bool clockwise)
        {

            foreach (var easyGridBuilderPro in m_GridManager.GetEasyGridBuilderProSystemsList())
            {
                if (clockwise)
                {
                    easyGridBuilderPro.SetInputBuildableObjectClockwiseRotation(true);
                }
                else
                {
                    easyGridBuilderPro.SetInputBuildableObjectCounterClockwiseRotation(true);
                }
            }
        }





        public void SetGridType(GridType type)
        {
            if (GridType != type)
            {
                var small = m_EasyGridBuilderPro_SmallCell;
                var large = Instance.m_EasyGridBuilderPro_LargeCell;
                switch (type)
                {
                    case GridType.Undefined:
                        small.gameObject.SetActive(false);
                        large.gameObject.SetActive(false);
                        break;
                    case GridType.Nothing:
                        small.gameObject.SetActive(false);
                        large.gameObject.SetActive(false);
                        GridManager.Instance.SetActiveGridSystem(small);
                        break;
                    case GridType.Large:
                        large.gameObject.SetActive(true);
                        small.gameObject.SetActive(false);
                        GridManager.Instance.SetActiveGridSystem(large);
                        break;
                    case GridType.Small:
                        large.gameObject.SetActive(false);
                        small.gameObject.SetActive(true);
                        GridManager.Instance.SetActiveGridSystem(small);
                        break;
                }
            }
        }





        private void OnDestroy()
        {
            spwanedEntityForEditor.Clear();
            m_CandidateAreaExpandProxies.Clear();
            m_CreatedBuildableEdgeObject.Clear();
        }





        public GridType GetGridTypeFromEasyGridBuilderPro(EasyGridBuilderPro gridBuilderPro)
        {
            if (gridBuilderPro == m_EasyGridBuilderPro_LargeCell)
            {
                return GridType.Large;
            }
            else if (gridBuilderPro == m_EasyGridBuilderPro_SmallCell)
            {
                return GridType.Small;
            }
            return GridType.Nothing;
        }



        private void OnGridObjectBoxPlacementFinalized(EasyGridBuilderPro easyGridBuilderPro)
        {
            if (easyGridBuilderPro.TryGetComponent<Entity_Test>(out var entity))
            {
                //if(entity.randomRotation)
            }
            //StartCoroutine(AreaExpand());
        }

        private void OnEdgeObjectBoxPlacementFinalized(EasyGridBuilderPro easyGridBuilderPro)
        {
            StartCoroutine(OnPostEdgeObjectBoxPlacementFinalized());
        }

        private IEnumerator OnPostEdgeObjectBoxPlacementFinalized()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            var cells = new HashSet<Element_LargeCell>();
            foreach (var entity in m_CreatedBuildableEdgeObject)
            {
                var containedSmallCells = ElementManager_Edge.Instance.GetContainedSmallCells(entity.edgeElement);
                foreach (var small in containedSmallCells)
                {
                    small.cellType = FlowTilemapSmallCellType.Wall;
                }
                var adjacentLargeCells = ElementManager_Edge.Instance.GetAdjacentLargeCells(entity.edgeElement);
                foreach (var cell in adjacentLargeCells)
                {
                    cells.Add(cell);
                }
                //ElementManager_Region.Instance.HandleWallPlacedIncremental(item);
            }
            ElementManager_Region.Instance.HandleWallsPlacedIncremental(cells);
            m_CreatedBuildableEdgeObject.Clear();
            //GridManager.Instance.TryGetBuildableEdgeObjectGhost
        }

        private IEnumerator AreaExpand()
        {
            //BindingService.MainGameViewModel.IsLandExpandMode = false;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            var cellList = new List<Element_LargeCell>();
            foreach (var item in m_CandidateAreaExpandProxies)
            {
                var cell = ElementManager_LargeCell.Instance.GetElement(item.transform.position);
                cellList.Add(cell);
            }

            foreach (var cell in cellList)
            {
                cell.Data.CellType = FlowTilemapCellType.Floor;
                CreateGroundForCellElement(cell);
            }

            var candidateEdges = new List<Element_Edge>();
            foreach (var cell in cellList)
            {
                var neighbors = ElementManager_LargeCell.Instance.GetCellNeighbors(cell.coord);

                var leftCell = neighbors[0];
                var leftEdge = ElementManager_Edge.Instance.GetLeftEdgeFromTileCoord(cell.coord);
                var leftRegion = ElementManager_Region.Instance.GetRegionFromLargeCoord(leftCell.coord);
                if (!cellList.Contains(leftCell) && leftEdge.GetWallEntity() == null && (leftCell.Data.CellType == FlowTilemapCellType.Custom || leftRegion == null))
                {
                    leftEdge.Data.EdgeType = FlowTilemapEdgeType.Fence;
                    candidateEdges.Add(leftEdge);
                }

                var upCell = neighbors[1];
                var upEdge = ElementManager_Edge.Instance.GetUpEdgeFromTileCoord(cell.coord);
                var upRegion = ElementManager_Region.Instance.GetRegionFromLargeCoord(upEdge.coord);
                if (!cellList.Contains(upCell) && upEdge.GetWallEntity() == null && (upCell.Data.CellType == FlowTilemapCellType.Custom || upRegion == null))
                {

                    upEdge.Data.EdgeType = FlowTilemapEdgeType.Fence;
                    candidateEdges.Add(upEdge);
                }

                var rightCell = neighbors[2];
                var rightEdge = ElementManager_Edge.Instance.GetRightEdgeFromTileCoord(cell.coord);
                var rightRegion = ElementManager_Region.Instance.GetRegionFromLargeCoord(rightEdge.coord);
                if (!cellList.Contains(rightCell) && rightEdge.GetWallEntity() == null && (rightCell.Data.CellType == FlowTilemapCellType.Custom || rightRegion == null))
                {
                    rightEdge.Data.EdgeType = FlowTilemapEdgeType.Fence;
                    candidateEdges.Add(rightEdge);
                }

                var downCell = neighbors[3];
                var downEdge = ElementManager_Edge.Instance.GetDownEdgeFromTileCoord(cell.coord);
                var downRegion = ElementManager_Region.Instance.GetRegionFromLargeCoord(downEdge.coord);
                if (!cellList.Contains(downCell) && downEdge.GetWallEntity() == null && (downCell.Data.CellType == FlowTilemapCellType.Custom || downRegion == null))
                {
                    downEdge.Data.EdgeType = FlowTilemapEdgeType.Fence;
                    candidateEdges.Add(downEdge);
                }
            }

            foreach (var item in candidateEdges)
            {
                CreateWallForEdgeElement(item);
            }


            for (int i = m_CandidateAreaExpandProxies.Count - 1; i >= 0; i--)
            {
                TryDestroyBuildableGridObject(m_CandidateAreaExpandProxies[i]);
            }

            m_CandidateAreaExpandProxies.Clear();

            //ProgrammaticMeshManager.Instance.UpdateMesh();
        }

        #region Create Entity
        private void CreateGroundForCellElement(Element_LargeCell cell)
        {
            var postion = CoordUtility.LargeCoordToWorldPosition(cell.coord);
            var rotation = RandomUtility.GetRandomRotation(cell.coord);
            //if (TryInitializeBuildableGridObjectSinglePlacement(postion, rotation, defaultGround, out var obj))
            //{
            //    var entity = obj.GetComponent<Entity_Ground>();
            //    entity.UpdateData();
            //}
        }

        private void CreateWallForEdgeElement(Element_Edge edge)
        {
            var postion = edge.worldPosition;
            Quaternion rotation;
            if (edge.Data.HorizontalEdge)
            {
                postion += new Vector3(0f, 0f, -1f);
                rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
            else
            {
                postion += new Vector3(-1f, 0f, 0f);
                rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
            }

            //if (TryInitializeBuildableEdgeObjectSinglePlacement(m_EasyGridBuilderPro_LargeCell, postion, rotation, defaultWall, out var obj))
            //{
            //    var entity = obj.GetComponent<Entity_Wall>();
            //    entity.UpdateData();
            //}
        }

        private void OnBuildableObjectPlaced(EasyGridBuilderPro easyGridBuilderPro, BuildableObject buildableObject)
        {
            if (!WorldManager.Instance.m_IsWorldReady) return;
            if (buildableObject.TryGetComponent<Entity>(out var entity))
            {
                entity.UpdateData();
                if (buildableObject is BuildableFreeObject buildableFreeObject)
                {

                }
                else if (buildableObject is BuildableEdgeObject buildableEdgeObject)
                {
                    if (entity is Entity_Wall wall)
                    {
                        var edge = wall.edgeElement;
                        edge.Data.EdgeType = FlowTilemapEdgeType.Wall;
                        m_CreatedBuildableEdgeObject.Add(wall);
                    }
                }
            }
            //buildableObject.transform.position += new Vector3(0f, -0.1f, 0f);
            //if (DungeonController.Instance.worldDataInited)
            //{
            //    //if (buildableObject is BuildableGridObject buildableGridObject)
            //    //{
            //    //    if (buildableObject.TryGetComponent<DevelopToolPanel>(out var areaExpandProxy))
            //    //    {
            //    //        m_CandidateAreaExpandProxies.Add(buildableGridObject);
            //    //    }
            //    //}






            //    //if (buildableObject is BuildableCornerObject buildableCornerObject)
            //    //{
            //    //    var edgeSO = BindingService.MainGameViewModel.ActiveEasyGridBuilderPro.GetActiveBuildableObjectSO() as BuildableEdgeObjectSO;
            //    //    if (edgeSO != null && !DirectionUtility.HasCornerConnectRightAngleEdges(buildableCornerObject.transform.position))
            //    //    {
            //    //        TryDestroyBuildableCornerObject(buildableCornerObject);
            //    //    }
            //    //}
            //}
            //Debug.Log("Rooms : " + ElementManager_Room.Instance.roomList.Count);
        }
        #endregion

        #region Placement
        public bool TryInitializeBuildableEdgeObjectSinglePlacement(EasyGridBuilderPro easyGridBuilderPro, Vector3 worldPosition, Quaternion rotation, BuildableEdgeObjectSO buildableEdgeObjectSO, out BuildableEdgeObject buildableGridObject, BuildableObjectSO.RandomPrefabs radomPrefabs = null)
        {
            //BindingService.MainGameViewModel.GameMode = GameMode.Structure;
            var fourDirectional = DirectionUtility.GetEdgeFourDirectionalRotationForWorld(rotation);
            if (radomPrefabs == null)
            {
                var coord = CoordUtility.WorldPositionToLargeCoord(worldPosition);
                radomPrefabs = RandomUtility.UpdateBuildableObjectSORandomPrefab(coord, buildableEdgeObjectSO);
            }

            if (easyGridBuilderPro.TryInitializeBuildableEdgeObjectSinglePlacement(worldPosition, buildableEdgeObjectSO, fourDirectional, false, true, true, 0, true, out buildableGridObject, radomPrefabs, null))
            {
                buildableGridObject.transform.parent = m_SpawnRoot;
                var children = buildableGridObject.GetComponentsInChildren<BuildableEdgeObject>();
                foreach (var item in children)
                {
                    item.SetOccupiedGridSystem(easyGridBuilderPro);
                }
                return true;
            }
            else
            {
                Debug.LogError($"Try Initialize Edge Error - ObjectOS : <{buildableEdgeObjectSO.objectName}> , Position <{worldPosition}>");
            }

            return false;
        }

        public bool TryInitializeBuildableGridObjectSinglePlacement(EasyGridBuilderPro easyGridBuilderPro, Vector3 worldPosition, FourDirectionalRotation dir, BuildableGridObjectSO buildableGridObjectSO, out BuildableGridObject buildableGridObject, BuildableObjectSO.RandomPrefabs radomPrefabs = null)
        {
            if (radomPrefabs == null)
            {
                var coord = CoordUtility.WorldPositionToLargeCoord(worldPosition);
                radomPrefabs = RandomUtility.UpdateBuildableObjectSORandomPrefab(coord, buildableGridObjectSO);
            }
            if (easyGridBuilderPro.TryInitializeBuildableGridObjectSinglePlacement(worldPosition, buildableGridObjectSO, dir, true, true, 0, true, out buildableGridObject, radomPrefabs, null))
            {
                buildableGridObject.transform.parent = m_SpawnRoot;
                if (buildableGridObject.GetIsActiveSceneObject(out _))
                {
                    Debug.Log(buildableGridObject.name);
                }
                return true;
            }
            else
            {
                Debug.LogError($"Place Grid Error - <>");
            }
            return false;
        }

        public bool TryInitializeBuildableCornerObjectSinglePlacement(EasyGridBuilderPro easyGridBuilderPro, Vector3 worldPosition, Quaternion rotation, BuildableCornerObjectSO buildableCornerObjectSO, out BuildableCornerObject buildableCornerObject, BuildableObjectSO.RandomPrefabs buildableObjectSORandomPrefab = null)
        {
            //BindingService.MainGameViewModel.GameMode = GameMode.Placement;
            var fourDirectional = DirectionUtility.GetEdgeFourDirectionalRotationForWorld(rotation);
            if (easyGridBuilderPro.TryInitializeBuildableCornerObjectSinglePlacement(worldPosition, buildableCornerObjectSO,
                 fourDirectional, EightDirectionalRotation.North, 0f, true, true, 0, true, out buildableCornerObject, buildableObjectSORandomPrefab, null))
            {
                buildableCornerObject.transform.parent = m_SpawnRoot;
                return true;
            }
            else
            {
                Debug.LogError($"Place Corner Error - <>");
            }

            return false;
        }

        public bool TryInitializeBuildableFreeObjectSinglePlacement(EasyGridBuilderPro easyGridBuilderPro, Vector3 worldPosition, Quaternion rotation, BuildableFreeObjectSO buildableFreeObjectSO, out BuildableFreeObject buildableObject, BuildableObjectSO.RandomPrefabs buildableObjectSORandomPrefab = null)
        {
            var fourDirectional = DirectionUtility.GetFreeFourDirectionalRotationForWorld(rotation);
            if (easyGridBuilderPro.TryInitializeBuildableFreeObjectSinglePlacement(worldPosition, buildableFreeObjectSO,
                 fourDirectional, EightDirectionalRotation.North, 0f, Vector3.zero, true, 0, true, out buildableObject, buildableObjectSORandomPrefab, null))
            {
                buildableObject.transform.parent = m_SpawnRoot;
                return true;
            }
            else
            {
                Debug.LogError($"Place Free Error - <>");
            }

            return false;
        }

        public bool TryDestroyBuildableGridObject(BuildableGridObject buildable)
        {
            if (GridManager.Instance.TryGetBuildableObjectDestroyer(out var destroyer))
            {
                if (destroyer.TryDestroyBuildableGridObject(buildable, true))
                {
                    return true;
                }
                else
                {
                    Debug.LogError($"TryDestroyGridObject faild : {buildable}");
                }
            }
            return false;
        }

        public bool TryDestroyBuildableFreeObject(BuildableFreeObject buildable)
        {
            if (GridManager.Instance.TryGetBuildableObjectDestroyer(out var destroyer))
            {
                if (destroyer.TryDestroyBuildableFreeObject(buildable, true))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryDestroyBuildableCornerObject(BuildableCornerObject buildable)
        {
            if (GridManager.Instance.TryGetBuildableObjectDestroyer(out var destroyer))
            {
                if (destroyer.TryDestroyBuildableCornerObject(buildable, true))
                {
                    return true;
                }
            }
            return false;
        }


        #endregion
    }
}
