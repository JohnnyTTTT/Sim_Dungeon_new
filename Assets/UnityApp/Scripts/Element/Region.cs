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
        public RoomType roomType;
        public HashSet<Element_LargeCell> containedLargeCells = new HashSet<Element_LargeCell>();
        public HashSet<Element_SmallCell> containedSmallCells = new HashSet<Element_SmallCell>();
        public bool isClosed;

        public BuildableGenAssets biome;
        public Color roomColor;
        public Bounds bounds;
        public Vector3 center;

        public void Init(string n, RoomType type)
        {
            name = n;
            roomType = type;
            roomColor = Random.ColorHSV();
        }

        public void AddSamllCell(Element_SmallCell cell)
        {
            containedSmallCells.Add(cell);
            ElementManager_Region.Instance.RegisterSamallCoord(cell.coord, this);
        }

        public void RemoveSmallCell(Element_SmallCell cell)
        {
            containedSmallCells.Remove(cell);
            ElementManager_Region.Instance.UnregisterSamallCoord(cell.coord, this);
        }

        public void AddLargeCell(Element_LargeCell cell)
        {
            containedLargeCells.Add(cell);
            ElementManager_Region.Instance.RegisterLargeCoord(cell.coord, this);
        }

        public void AddLargeCells(HashSet<Element_LargeCell> cells)
        {
            foreach (var cell in cells)
            {
                AddLargeCell(cell);
            }
        }

        public void RemoveLargeCell(Element_LargeCell cell)
        {
            containedLargeCells.Remove(cell);
            ElementManager_Region.Instance.UnregisterLargeCoord(cell.coord, this);
        }

        public void RemoveLargeCells(HashSet<Element_LargeCell> cells)
        {
            foreach (var cell in cells)
            {
                RemoveLargeCell(cell);
            }
        }

        public void CalculateBounds()
        {
            // ��ʼ�� bounds
            bounds = new Bounds(containedLargeCells.First().worldPosition, Vector3.zero);

            // �������и���
            foreach (var cell in containedLargeCells)
            {
                bounds.Encapsulate(cell.worldPosition);
            }

            // Y����Ժ��Ի򱣳�Ϊ0
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
            foreach (var item in containedLargeCells)
            {
                GizmoUnitily.DrawTwoSizeCube(item.worldPosition, roomColor, true);
}
            Color.RGBToHSV(roomColor, out float h, out float s, out float v);
            v *= 0.3f; 
            var darker = Color.HSVToRGB(h, s, v);
            foreach (var item in containedSmallCells)
            {
                GizmoUnitily.DrawOneSizeCube(item.worldPosition, darker, true);
            }
            GizmoUnitily.DrawLabel(center, name);
        }









#endif
    }
}
