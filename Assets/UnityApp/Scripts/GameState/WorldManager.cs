using DungeonArchitect;
using DungeonArchitect.Flow.Domains.Tilemap;
using Loxodon.Framework.Contexts;
using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Johnny.SimDungeon.ElementManager_Edge;
using static Johnny.SimDungeon.ElementManager_LargeCell;

namespace Johnny.SimDungeon
{
    public enum DevOptions
    {
        None,
        Nothing,
        NoCreate,
        NoData,
    }

    public enum DevDungenCreateMode
    {
        Ground,
        Underground,
        Both
    }
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<WorldManager>();
                }
                return s_Instance;
            }

        }
        private static WorldManager s_Instance;

        public static event Action OnWorldCreated;
        
        [Title("Dev")]
        public DevOptions devOption;
        public DevDungenCreateMode dungenCreateMode;
        public bool loadMode;


        [Title("Grid System")]
        public DisablerController disablerController_SmallCell;
        public DisablerController disablerController_LargeCell;

        public Vector2Int largeTilemapSize;
        public Vector2Int smallTilemapSize;

        public bool m_IsLargeGridReady;
        public bool m_IsSmallGridReady;
        public bool m_IsGroundDeogunReady;
        public bool m_IsWorldReady;
        private MainGameViewModel m_MainGameViewModel;

        private void Start()
        {
            if (devOption == DevOptions.Nothing)
            {
                OnWorldCreated?.Invoke();
                return;
            }
            DungeonBuildListener.OnPostDungeonBuildAction += OnPostDungeonBuildAction;
            //DungeonController.Instance.OnPostDungeonBuildAction += OnPostDungeonBuild;
            EasyGridBuilderPro.OnGridSystemCreated += OnGridSystemCreated;
            var serviceContainer = Context.GetApplicationContext().GetContainer();
            m_MainGameViewModel = serviceContainer.Resolve<MainGameViewModel>();
            switch (dungenCreateMode)
            {
                case DevDungenCreateMode.Ground:
                    StartCoroutine(CreateGroundWorld());
                    break;
                case DevDungenCreateMode.Underground:
                    StartCoroutine(CreateUndergroundWorld());
                    break;
                case DevDungenCreateMode.Both:
                    break;
                default:
                    break;
            }
        }

        private void OnPostDungeonBuildAction(Dungeon dungeon, DungeonModel dungeonModel, LevelMarkerList sockets)
        {
            if (dungeon == DungeonController.Instance.dungeon)
            {
                m_IsGroundDeogunReady = true;
            }
        }

        private void OnGridSystemCreated(EasyGridBuilderPro easyGridBuilderPro)
        {
            if (easyGridBuilderPro == SpawnManager.Instance.m_EasyGridBuilderPro_LargeCell)
            {
                m_IsLargeGridReady = true;
                var gridLarge = easyGridBuilderPro.GetActiveGrid() as GridXZ;
                largeTilemapSize = new Vector2Int(gridLarge.GetWidth(), gridLarge.GetLength());
            }
            else if (easyGridBuilderPro == SpawnManager.Instance.m_EasyGridBuilderPro_SmallCell)
            {
                m_IsSmallGridReady = true;
                var gridSmall = easyGridBuilderPro.GetActiveGrid() as GridXZ;
                smallTilemapSize = new Vector2Int(gridSmall.GetWidth(), gridSmall.GetLength());
            }
        }


        private IEnumerator CreateUndergroundWorld()
        {

            yield return new WaitForEndOfFrame();
            RandomUtility.SetSeed((int)DungeonController.Instance.undergroundDungeon.Config.Seed);
            GameStateManager.Instance.Initialize();
            while (!m_IsLargeGridReady || !m_IsSmallGridReady)
            {
                yield return null;
            }
            Debug.Log("[-----System-----] : EGB 加载完毕");
            yield return new WaitForEndOfFrame();
            DungeonController.Instance.DestroyUndergroundDungeon();

            yield return new WaitForEndOfFrame();
            DungeonController.Instance.BuildUndergroundDungeon();

        }

        private IEnumerator CreateGroundWorld()
        {
            yield return new WaitForEndOfFrame();

            Debug.Log("[-----System-----] : 世界开始加载");
            RandomUtility.SetSeed((int)DungeonController.Instance.dungeon.Config.Seed);
            GameStateManager.Instance.Initialize();

            while (!m_IsLargeGridReady || !m_IsSmallGridReady)
            {
                yield return null;
            }
            Debug.Log("[-----System-----] : EGB 加载完毕");

            yield return new WaitForEndOfFrame();
            DungeonController.Instance.DestroyGroundDungeon();

            yield return new WaitForEndOfFrame();
            DungeonController.Instance.BuildGroundDungeon();

            if (!loadMode)
            { 
            
            }
            while (!m_IsGroundDeogunReady)
            {
                yield return null;
            }
            Debug.Log("[-----System-----] : 地牢创建完毕");

            yield return new WaitForEndOfFrame();
            var gridFlowDungeonModel = DungeonController.Instance.dungeonModel;

            //LargeCellDatas
            Debug.Log(gridFlowDungeonModel.Tilemap.Cells.Count());
            ElementManager_LargeCell.Instance.Initialize(gridFlowDungeonModel.Tilemap.Cells);

            //EdgeDatas
            ElementManager_Edge.Instance.Initialize(gridFlowDungeonModel.Tilemap.Edges);


            ElementManager_SmallCell.Instance.Initialize(SpawnManager.Instance.m_EasyGridBuilderPro_SmallCell);
            ElementManager_Region.Instance.Initialize();

            ElementManager_SmallCell.Instance.PostInit();
            ElementManager_Region.Instance.PostInit();

            Debug.Log("[-----System-----] : 数据创建完毕");


            ProgrammaticMeshManager.Instance.Init();
            SpawnManager.Instance.Init();

            var disablerLargeCell = new HashSet<Vector2Int>();
            foreach (var item in ElementManager_LargeCell.Instance.GetAllElements())
            {
                if (item.cellType != LargelCellType.Floor)
                {
                    disablerLargeCell.Add(item.coord);
                }
            }
            disablerController_LargeCell.AddDisablerCells(disablerLargeCell);

            var disablerSmallCell = new HashSet<Vector2Int>();
            foreach (var item in ElementManager_SmallCell.Instance.GetAllElements())
            {
                if (item.cellType != SmallCellType.Floor)
                {
                    disablerSmallCell.Add(item.coord);
                }
            }
            disablerController_SmallCell.AddDisablerCells(disablerSmallCell);

            //GameStateManager.Instance.ChangeState(GameState.Default);
            m_IsWorldReady = true;
            OnWorldCreated?.Invoke();
            Debug.Log("[-----System-----] : 世界创建完毕");
        }

        [Button]
        public void Save()
        {
            EasyGridBuilderProSaveSystem.Save();
            //if (GridManager.Instance.TryGetGridSaveAndLoadManager(out var gridSaveAndLoadManager))
            //{
            //    gridSaveAndLoadManager.Save();
            //}
        }
        [Button]
        public void Load()
        {
            EasyGridBuilderProSaveSystem.Load();
            //if (GridManager.Instance.TryGetGridSaveAndLoadManager(out var gridSaveAndLoadManager))
            //{
            //    gridSaveAndLoadManager.Save();
            //}
        }

        private void OnDestroy()
        {
            DungeonController.Instance.DestroyGroundDungeon();
            ElementManager_LargeCell.Instance.Dispose();
            ElementManager_Edge.Instance.Dispose();
            ElementManager_SmallCell.Instance.Dispose();
            ElementManager_Region.Instance.Dispose();
        }
    }
}
