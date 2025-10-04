using System;
using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
    [Serializable]
    public struct ElementsSerializationData
    {
        public LargeElementsSaveData largeElementsSaveData;
        public SmallElementsSaveData smallElementsSaveData;
        public LargeEdgesSaveData largeEdgesSaveData;
        public RegionsSaveData regionsSaveData;
    }

    [Serializable]
    public struct LargeElementsSaveData
    {
        public List<LargeElementSaveData> largeElementsSaveData;
    }

    [Serializable]
    public class LargeElementSaveData
    {
        public Vector2Int coord;
        public LargelCellType cellType;
    }

    [Serializable]
    public struct SmallElementsSaveData
    {
        public List<SmallElementSaveData> smallElementSaveDatas;
    }

    [Serializable]
    public class SmallElementSaveData
    {
        public Vector2Int coord;
        public SmallCellType smallCellType;
    }

    [Serializable]
    public struct LargeEdgesSaveData
    {
        public List<LargeEdgeSaveData> largeEdgeSaveDatas;
    }

    [Serializable]
    public class LargeEdgeSaveData
    {
        public Vector2Int coord;
        public EdgeType edgeType;
        public bool horizontalEdge;
    }

    [Serializable]
    public struct RegionsSaveData
    {
        public List<RegionSaveData> regionSaveDatas;
    }

    [Serializable]
    public class RegionSaveData
    {
        public string name;
        public RoomType roomType;
        public Color roomColor;

        public List<Vector2Int> containedLargeCells = new List<Vector2Int>();
    }
}
