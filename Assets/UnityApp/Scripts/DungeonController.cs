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

        public event Action OnPostDungeonBuildAction;

        public Dungeon dungeon;
        public GridFlowDungeonConfig dungeonConfig;
        public GridFlowDungeonModel dungeonModel;
        public GridFlowDungeonQuery gridFlowDungeonQuery;

        private RuntimeSimSceneObjectInstantiator m_RuntimeSimSceneObjectInstantiator;


        private void Awake()
        {
            m_RuntimeSimSceneObjectInstantiator = new RuntimeSimSceneObjectInstantiator();
        }

        public void BuildDungeon()
        {
            dungeon.Build(m_RuntimeSimSceneObjectInstantiator);
        }

        public void DestroyDungeon()
        {
            dungeon.DestroyDungeon();
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
