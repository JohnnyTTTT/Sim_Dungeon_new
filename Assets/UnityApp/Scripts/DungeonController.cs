using DungeonArchitect;
using DungeonArchitect.Builders.GridFlow;
#if UNITY_EDITOR
using DungeonArchitect.Editors;
#endif
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Impl.GridFlow;
using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Johnny.SimDungeon
{
    public class DungeonController : DungeonEventListener
    {
        public static DungeonController Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<DungeonController>();
                }
                return s_Instance;
            }

        }
        private static DungeonController s_Instance;

        public event Action OnWorldCreated;

        public Dungeon dungeon;
        public GridFlowDungeonConfig dungeonConfig;
        public GridFlowDungeonModel dungeonModel;
        public GridFlowDungeonBuilder gridFlowDungeonBuilder;
        public PooledDungeonSceneProvider pooledDungeonSceneProvider;
        public GridFlowDungeonQuery gridFlowDungeonQuery;
        public GridFlowMinimap gridFlowMinimap;
        public BuildingItemSpawnListener buildingItemSpawnListener;
        public EasyGridBuilderProController easyGridBuilderProController;

        [Title("Disabler")]
        public DisablerController disablerController_SmallCell;
        public DisablerController disablerController_LargeCell;

        public bool worldDataInited;
        private RuntimeSimSceneObjectInstantiator m_RuntimeSimSceneObjectInstantiator;
        public Vector2Int largeTilemapSize;
        public Vector2Int smallTilemapSize;

        private void Awake()
        {
            RandomUtility.SetSeed((int)Instance.dungeon.Config.Seed);
            m_RuntimeSimSceneObjectInstantiator = new RuntimeSimSceneObjectInstantiator();
        }

        private void Start()
        {
            StartCoroutine(BuildDungeonPlaying());
        }

        private IEnumerator BuildDungeonPlaying()
        {
            Debug.Log("[-----System-----] : 系统创建开始");
            GameStateManager.Instance.Initialize();
            BindingService.MainGameViewModel.GameState = GameState.Loading;
            yield return new WaitForEndOfFrame();
            DestroyDungeon();
            yield return new WaitForEndOfFrame();
            dungeon.Build(m_RuntimeSimSceneObjectInstantiator);
            yield return new WaitForEndOfFrame();
            largeTilemapSize = new Vector2Int(dungeonModel.Tilemap.Width, dungeonModel.Tilemap.Height);
            var grid = SpawnManager.Instance.m_EasyGridBuilderPro_SmallCell.GetActiveGrid() as GridXZ;
            smallTilemapSize = new Vector2Int(grid.GetWidth(), grid.GetLength());
            worldDataInited = true;
            EntityInit();


            //disablerController_LargeCell.Init();
            //disablerController_SmallCell.Init();

            //var disablerLargeCell = new HashSet<Vector2Int>();

            //foreach (var item in ElementManager_LargeCell.Instance.GetAllElements())
            //{
            //    if (item.Data.CellType != FlowTilemapCellType.Floor)
            //    {
            //        disablerLargeCell.Add(item.coord);
            //        //foreach (var small in item.containedSmallCells)
            //        //{
            //        //    if (small != null)
            //        //    {
            //        //        disablerSmallCell.Add(small.coord);
            //        //    }
            //        //}
            //    }
            //}

            //var disablerSmallCell = new HashSet<Vector2Int>();
            //foreach (var item in ElementManager_SmallCell.Instance.GetAllElements())
            //{
            //    if (item.isBuildingValid)
            //    {
            //        disablerSmallCell.Add(item.coord);
            //    }
            //}
            //foreach (var edge in ElementManager_Edge.Instance.GetAllElements())
            //{
            //    if (edge.Data.EdgeType != FlowTilemapEdgeType.Empty)
            //    {
            //        foreach (var small in edge.containedSmallCells)
            //        {
            //            disablerSmallCell.Add(small.coord);
            //        }
            //    }
            //}


            //disablerController_LargeCell.AddDisablerCells(disablerLargeCell);
            //disablerController_SmallCell.AddDisablerCells(disablerSmallCell);

            BindingService.MainGameViewModel.GameState = GameState.Default;
            BindingService.MainGameViewModel.GridType = GridType.Nothing;
            //GridManager.Instance.SetActiveGridModeInAllGrids(GridMode.SelectMode);

            //disablerController_LargeCell.Init();
            OnWorldCreated?.Invoke();
            Debug.Log("[-----System-----] : 系统创建完毕");

        }

        public void BuildDungeonEditor()
        {
            dungeon.Build(new RuntimeSimSceneObjectInstantiator());
        }

        public void DestroyDungeon()
        {
            dungeon.DestroyDungeon();
          
        }

        private void DataInit()
        {
            var gridFlowDungeonModel = dungeonModel;
            ElementManager_LargeCell.Instance.Init(gridFlowDungeonModel.Tilemap.Cells);
            ElementManager_Edge.Instance.Init(gridFlowDungeonModel.Tilemap.Edges);
            ElementManager_SmallCell.Instance.Init(SpawnManager.Instance.m_EasyGridBuilderPro_SmallCell);
            ElementManager_Region.Instance.Init();


            ElementManager_LargeCell.Instance.PostInit();
            ElementManager_Edge.Instance.PostInit();
            ElementManager_SmallCell.Instance.PostInit();
            ElementManager_Region.Instance.PostInit();

            if (Application.isPlaying)
            {
                MainUIManager.Instance.Init();
            }

            Debug.Log("[-----System-----] : 数据初始化");
        }

        private void EntityInit()
        {
            ProgrammaticMeshManager.Instance.Init();
            SpawnManager.Instance.Init();
            Debug.Log("[-----System-----] : 实体初始化");
        }


        public override void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model)
        {
        }

        public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers)
        {
            DataInit();
        }

        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            //var entities = FindObjectsOfType<Entity>();
            //foreach (var item in entities)
            //{
            //    item.UpdateData();
            //}


            Debug.Log("[-----System-----] : 地牢创建完毕");
        }

        public override void OnDungeonDestroyed(Dungeon dungeon)
        {
            ElementManager_LargeCell.Instance.UnInit();
            ElementManager_Edge.Instance.UnInit();
            ElementManager_Region.Instance.Dispose();
            ElementManager_SmallCell.Instance.UnInit();
            SpawnManager.Instance.UnInit();
            ProgrammaticMeshManager.Instance.Dispose();

        }
    }
}
