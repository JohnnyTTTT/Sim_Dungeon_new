using DungeonArchitect;
using DungeonArchitect.Flow.Domains.Tilemap;
using Loxodon.Framework.Contexts;
using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
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

        [Title("Grid System")]
        public DisablerController disablerController_SmallCell;
        public DisablerController disablerController_LargeCell;

        public Vector2Int largeTilemapSize;
        public Vector2Int smallTilemapSize;

        public bool m_IsLargeGridReady;
        public bool m_IsSmallGridReady;
        public bool m_IsDeogunReady;
        public bool m_IsWorldReady;
        private MainGameViewModel m_MainGameViewModel;

        private void Start()
        {
            DungeonController.Instance.OnPostDungeonBuildAction += OnPostDungeonBuild;
            EasyGridBuilderPro.OnGridSystemCreated += OnGridSystemCreated;
            var serviceContainer = Context.GetApplicationContext().GetContainer();
            m_MainGameViewModel = serviceContainer.Resolve<MainGameViewModel>();
            StartCoroutine(LoadWorld());
        }

        private void OnPostDungeonBuild()
        {
            m_IsDeogunReady = true;
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

        private IEnumerator LoadWorld()
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
            DungeonController.Instance.DestroyDungeon();

            yield return new WaitForEndOfFrame();
            DungeonController.Instance.BuildDungeon();

            while (!m_IsDeogunReady)
            {
                yield return null;
            }
            Debug.Log("[-----System-----] : 地牢创建完毕");

            yield return new WaitForEndOfFrame();
            var gridFlowDungeonModel = DungeonController.Instance.dungeonModel;
            ElementManager_LargeCell.Instance.Init(gridFlowDungeonModel.Tilemap.Cells);
            ElementManager_Edge.Instance.Init(gridFlowDungeonModel.Tilemap.Edges);
            ElementManager_SmallCell.Instance.Init(SpawnManager.Instance.m_EasyGridBuilderPro_SmallCell);
            ElementManager_Region.Instance.Init();

            ElementManager_LargeCell.Instance.PostInit();
            ElementManager_Edge.Instance.PostInit();
            ElementManager_SmallCell.Instance.PostInit();
            ElementManager_Region.Instance.PostInit();

            Debug.Log("[-----System-----] : 数据创建完毕");


            ProgrammaticMeshManager.Instance.Init();
            SpawnManager.Instance.Init();

            var disablerLargeCell = new HashSet<Vector2Int>();
            foreach (var item in ElementManager_LargeCell.Instance.GetAllElements())
            {
                if (item.Data.CellType != FlowTilemapCellType.Floor)
                {
                    disablerLargeCell.Add(item.coord);
                }
            }
            disablerController_LargeCell.AddDisablerCells(disablerLargeCell);

            var disablerSmallCell = new HashSet<Vector2Int>();
            foreach (var item in ElementManager_SmallCell.Instance.GetAllElements())
            {
                if (item.cellType != FlowTilemapSmallCellType.Floor)
                {
                    disablerSmallCell.Add(item.coord);
                }
            }
            disablerController_SmallCell.AddDisablerCells(disablerSmallCell);

            GameStateManager.Instance.ChangeState(GameState.Default);
            m_IsWorldReady = true;
            Debug.Log("[-----System-----] : 世界创建完毕");
        }
    }
}
