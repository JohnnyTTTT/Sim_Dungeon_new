using DungeonArchitect;
using DungeonArchitect.Flow.Domains.Tilemap;
using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Johnny.SimDungeon
{
    public enum LargelCellType
    {
        Empty,
        Floor,
    }

    public class Element_LargeCell : Element
    {
        public LargelCellType cellType;

        private Entity_Floor m_Ground;
        public Entity_Ceiling ceiling;

        public Element_Edge horizontalEdge;
        public Element_Edge verticalEdge;

        public Entity_SubEdge horizontalSubEdge;
        public Entity_SubEdge verticalSubEdge;

        public Vector3 worldPosition;
        public Vector2Int coord;


        public Element_LargeCell(Vector2Int position)
        {
            coord = position;
            worldPosition = CoordUtility.LargeCoordToWorldPosition(coord);
        }

        public void SetGroundEntity(Entity_Floor ground)
        {
            m_Ground = ground;
            //var valid = 
            //for (int i = 0; i < length; i++)
            //{

            //}
            //DungeonController.Instance.disablerController_SmallCell.RemoveDisablerCells(containedSmallCells);
            //foreach (var item in containedSmallCells)
            //{
            //    item.isBuildingValid = true;
            //}
        }

        public Entity_Floor GetGroundEntity()
        {
            return m_Ground;
        }

        public void DrawGizmos()
        {
            GizmoUnitily.DrawTwoSizeCube(worldPosition, Color.green, true);
            GizmoUnitily.DrawLabel(coord, $"{coord} ");
        }

        public override string ToString()
        {
            return $"LargeCell_<{coord}>_<{cellType}>";
        }

    }

    public class ElementManager_LargeCell : ElementManager<Element_LargeCell>
    {
        public static ElementManager_LargeCell Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<ElementManager_LargeCell>();
                }
                return s_Instance;
            }

        }
        private static ElementManager_LargeCell s_Instance;

        public Vector2Int drawGizmosCoord;
        public void Initialize(FlowTilemapCellDatabase cells)
        {
            foreach (var cell in cells)
            {
                var coord = new Vector2Int(cell.TileCoord.x, cell.TileCoord.y);
                var element = new Element_LargeCell(coord);
                element.cellType = cell.CellType == FlowTilemapCellType.Floor ? LargelCellType.Floor : LargelCellType.Empty;
                map[coord] = element;
            }
            Debug.Log($"[-----System-----] : DataManager Cell inited , Cell count <{map.Count}>");
        }


        public void Dispose()
        {
            map?.Clear();
        }

        public Element_LargeCell[] GetCellNeighbors(Vector2Int coord)
        {
            var neighbors = new Element_LargeCell[4];
            neighbors[0] = GetLeftCellFromCoord(coord);
            neighbors[1] = GetUpCellFromCoord(coord);
            neighbors[2] = GetRightCellFromCoord(coord);
            neighbors[3] = GetDownCellFromCoord(coord);
            return neighbors;
        }

        public Element_LargeCell GetLeftCellFromCoord(Vector2Int coord)
        {
            return GetElement(coord + DirectionUtility.LEFT);
        }

        public Element_LargeCell GetUpCellFromCoord(Vector2Int coord)
        {
            return GetElement(coord + DirectionUtility.UP);
        }

        public Element_LargeCell GetRightCellFromCoord(Vector2Int coord)
        {
            return GetElement(coord + DirectionUtility.RIGHT);
        }


        public Element_LargeCell GetDownCellFromCoord(Vector2Int coord)
        {
            return GetElement(coord + DirectionUtility.DOWN);
        }


        private void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                foreach (var item in map)
                {
                    if (item.Value.cellType == LargelCellType.Floor)
                    {
                        item.Value.DrawGizmos();
                    }
                }
            }
        }
    }
}