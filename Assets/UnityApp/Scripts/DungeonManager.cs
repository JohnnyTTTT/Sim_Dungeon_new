using DungeonArchitect;
using DungeonArchitect.Builders.GridFlow;
#if UNITY_EDITOR
using DungeonArchitect.Editors;
#endif
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Impl.GridFlow;
using DungeonArchitect.Utils;
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
    [Serializable]
    public class DungeonConfigs
    {
        public static Vector3 GridSize = new Vector3(2f, 4f, 2f);
        public Vector3 basePosition;

        public Dungeon dungeon;
        public GridFlowDungeonConfig dungeonConfig;
        public GridFlowDungeonModel dungeonModel;
        public GridFlowDungeonQuery gridFlowDungeonQuery;

        public IntVector2 WorldPositionToTilemapCoord(Vector3 worldPosition)
        {
            var localWorld = worldPosition - basePosition;
            var localTileF = MathUtils.Divide(localWorld, GridSize);
            return new IntVector2(Mathf.FloorToInt(localTileF.x), Mathf.FloorToInt(localTileF.z));
        }

        public Vector3 TileCoordToWorldCoord(IntVector2 tileCoord)
        {
            return basePosition + Vector3.Scale(new Vector3(tileCoord.x + 0.5f, 0, tileCoord.y + 0.5f), GridSize);
        }

        public int GetSeed()
        {
            return (int)dungeonConfig.Seed; 
        }
    }

    public class DungeonManager : DungeonEventListener
    {
        public static DungeonManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<DungeonManager>();
                }
                return s_Instance;
            }

        }
        private static DungeonManager s_Instance;



        public event Action OnPostDungeonBuildAction;

        [Title("Ground")]
        public DungeonConfigs groundDungeon;

        [Title("Underground")]
        public DungeonConfigs undergroundDungeon;


        private RuntimeSimSceneObjectInstantiator m_RuntimeSimSceneObjectInstantiator;
        private RuntimeSimSceneObjectInstantiator m_RuntimeUnderSceneObjectInstantiator;


        private void Awake()
        {
            m_RuntimeSimSceneObjectInstantiator = new RuntimeSimSceneObjectInstantiator(Vector3.zero);
            m_RuntimeUnderSceneObjectInstantiator = new RuntimeSimSceneObjectInstantiator(new Vector3(0f, -50f, 0f));
        }



        public void BuildUndergroundDungeonEditor()
        {
            undergroundDungeon.dungeon.Build(new RuntimeSimSceneObjectInstantiator(new Vector3(0f, -50f, 0f)));
        }

        public void BuildUndergroundDungeon()
        {
            undergroundDungeon.dungeon.Build(new RuntimeSimSceneObjectInstantiator(new Vector3(0f, -50f, 0f)));
        }

        public void DestroyUndergroundDungeon()
        {
            undergroundDungeon.dungeon.DestroyDungeon();
        }

        public void BuildDungeonEditor()
        {
            groundDungeon.dungeon.Build(new RuntimeSimSceneObjectInstantiator(Vector3.zero));
        }

        public void BuildGroundDungeon()
        {
            groundDungeon.dungeon.Build(m_RuntimeSimSceneObjectInstantiator);
        }

        public void DestroyGroundDungeon()
        {
            groundDungeon.dungeon.DestroyDungeon();
        }

        public override void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model)
        {
        }


        public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers)
        {
        }


        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            OnPostDungeonBuildAction?.Invoke();
        }

        public override void OnDungeonDestroyed(Dungeon dungeon)
        {
            //ElementManager_LargeCell.Instance.UnInit();
            //ElementManager_Edge.Instance.UnInit();
            //ElementManager_Region.Instance.Dispose();
            //ElementManager_SmallCell.Instance.UnInit();
            //SpawnManager.Instance.UnInit();
            //ProgrammaticMeshManager.Instance.Dispose();

        }
    }
}
