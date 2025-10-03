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
        [Title("Dev")]
        public DevDungenCreateMode dungenCreateMode;

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
            switch (dungenCreateMode)
            {
                case DevDungenCreateMode.Ground:
                    StartCoroutine(CreateGroundWorld());
                    break;
                case DevDungenCreateMode.Underground:
                    //StartCoroutine(CreateUndergroundWorld());
                    break;
                case DevDungenCreateMode.Both:
                    break;
                default:
                    break;
            }
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

        private IEnumerator CreateGroundWorld()
        {
            yield return new WaitForEndOfFrame();

            Debug.Log("[-----System-----] : ���翪ʼ����");
            RandomUtility.SetSeed((int)DungeonController.Instance.dungeon.Config.Seed);
            GameStateManager.Instance.Initialize();

            while (!m_IsLargeGridReady || !m_IsSmallGridReady)
            {
                yield return null;
            }
            Debug.Log("[-----System-----] : EGB �������");

            yield return new WaitForEndOfFrame();
            DungeonController.Instance.DestroyGroundDungeon();

            yield return new WaitForEndOfFrame();
            DungeonController.Instance.BuildGroundDungeon();

            while (!m_IsDeogunReady)
            {
                yield return null;
            }
            Debug.Log("[-----System-----] : ���δ������");

            yield return new WaitForEndOfFrame();
            var gridFlowDungeonModel = DungeonController.Instance.dungeonModel;
            ElementManager_LargeCell.Instance.Initialize(gridFlowDungeonModel.Tilemap.Cells);
            ElementManager_Edge.Instance.Initialize(gridFlowDungeonModel.Tilemap.Edges);
            ElementManager_SmallCell.Instance.Initialize(SpawnManager.Instance.m_EasyGridBuilderPro_SmallCell);
            ElementManager_Region.Instance.Initialize();

            ElementManager_Edge.Instance.PostInit();
            ElementManager_Region.Instance.PostInit();

            Debug.Log("[-----System-----] : ���ݴ������");


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
            Debug.Log("[-----System-----] : ���紴�����");
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
