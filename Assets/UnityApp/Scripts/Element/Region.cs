using DungeonArchitect;
using DungeonArchitect.Flow.Domains.Tilemap;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.UI.Image;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Johnny.SimDungeon
{
    public class Region : Element
    {
        public string name;
        public Vector3 center;
        public RoomType roomType;
        public Color roomColor;

        public HashSet<Vector2Int> containedLargeCells = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> containedSmallCells = new HashSet<Vector2Int>();



        public void Init(string n, RoomType type)
        {
            name = n;
            roomType = type;
            roomColor = Random.ColorHSV();
        }

        public void AddSamllCell(Vector2Int coord)
        {
            containedSmallCells.Add(coord);
            ElementManager_Region.Instance.RegisterSamallCoord(coord, this);
        }

        public void RemoveSmallCell(Vector2Int cell)
        {
            containedSmallCells.Remove(cell);
            ElementManager_Region.Instance.UnregisterSamallCoord(cell, this);
        }

        public void AddLargeCell(Vector2Int cell)
        {
            containedLargeCells.Add(cell);
            ElementManager_Region.Instance.RegisterLargeCoord(cell, this);
        }

        public void AddLargeCells(HashSet<Vector2Int> cells)
        {
            foreach (var cell in cells)
            {
                AddLargeCell(cell);
            }
        }

        public void RemoveLargeCell(Vector2Int cell)
        {
            containedLargeCells.Remove(cell);
            ElementManager_Region.Instance.UnregisterLargeCoord(cell, this);
        }

        public void RemoveLargeCells(HashSet<Vector2Int> cells)
        {
            foreach (var cell in cells)
            {
                RemoveLargeCell(cell);
            }
        }

        public void CalculateBounds()
        {
            if (containedLargeCells.Count <= 0) return;

            // 初始化 bounds
            var worldPosition = CoordUtility.LargeCoordToWorldPosition(containedLargeCells.First());
            var bounds = new Bounds(worldPosition, Vector3.zero);

            // 包含所有格子
            foreach (var cell in containedLargeCells)
            {
                worldPosition = CoordUtility.LargeCoordToWorldPosition(cell);
                bounds.Encapsulate(worldPosition);
            }

            // Y轴可以忽略或保持为0
            center = new Vector3(bounds.center.x, 0f, bounds.center.z);
        }

        public void Clear()
        {
            containedLargeCells.Clear();
            containedSmallCells.Clear();
        }



        public override string ToString()
        {
            return name;
        }

#if UNITY_EDITOR
        public void DrawGizmos()
        {
            foreach (var item in containedSmallCells)
            {
                var worldPosition = CoordUtility.SmallCoordToWorldPosition(item);
                GizmoUnitily.DrawOneSizeCube(worldPosition, roomColor, true);
            }

            Color.RGBToHSV(roomColor, out float h, out float s, out float v);
            v *= 0.5f;
            var darker = Color.HSVToRGB(h, s, v);
            foreach (var item in containedLargeCells)
            {
                var worldPosition = CoordUtility.LargeCoordToWorldPosition(item);
                GizmoUnitily.DrawTwoSizeCube(worldPosition, darker, true);
            }
            GizmoUnitily.DrawLabel(center, name);
        }









#endif
    }
}
