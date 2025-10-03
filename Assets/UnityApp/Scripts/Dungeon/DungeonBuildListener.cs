using DungeonArchitect;
using System;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class DungeonBuildListener : DungeonEventListener
    {
        public static event Action<Dungeon, DungeonModel, LevelMarkerList> OnPostDungeonBuildAction;
        public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers)
        {
            OnPostDungeonBuildAction?.Invoke(dungeon, model, markers);
        }
    }
}
