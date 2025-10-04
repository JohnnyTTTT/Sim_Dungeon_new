using DungeonArchitect;
using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

namespace Johnny.SimDungeon
{
    public enum SmallCellType
    {
        Empty,
        Floor,
        Wall,
    }

    public class Element_SmallCell : Element
    {
        //public Element_SmallCell[] neighbors = new Element_SmallCell[4];
        public Vector2Int coord;
        public SmallCellType cellType
        {
            get
            {
                return cellType1;
            }
            set
            {
                cellType1 = value;
            }
        }
        public SmallCellType cellType1;

        //public Element_Edge wall;
        //public Element_LargeCell parentCell;

        public Element_SmallCell(Vector2Int vector)
        {
            coord = vector;
        }

        public override string ToString()
        {
            return cellType.ToString();
        }

        public void DrawGizmos()
        {
            var color = (cellType) switch
            {
                SmallCellType.Empty => Color.black,
                SmallCellType.Floor => Color.blue,
                SmallCellType.Wall => Color.red,
                //SmallCellType.Door => Color.green,
            };
            var worldPosition = CoordUtility.SmallCoordToWorldPosition(coord);
            GizmoUnitily.DrawOneSizeCube(worldPosition, color, true);
        }
    }

    public class ElementManager_SmallCell : ElementManager<Element_SmallCell>
    {
        public static ElementManager_SmallCell Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<ElementManager_SmallCell>();
                }
                return s_Instance;
            }
        }
        private static ElementManager_SmallCell s_Instance;

        public void Initialize(EasyGridBuilderPro easyGridBuilder)
        {
            for (int x = 0; x < easyGridBuilder.GetGridWidth(); x++)
            {
                for (int z = 0; z < easyGridBuilder.GetGridLength(); z++)
                {
                    var position = new Vector2Int(x, z);
                    var newData = new Element_SmallCell(position);
                    map.Add(newData.coord, newData);
                }
            }
            Debug.Log($"[-----System-----] : ElementManager_SamllCell data inited , SmallCell count <{map.Count}>");
        }

        public void LoadElements(List<SmallElementSaveData> smallElementSaveDatas)
        {
            foreach (var cellSaveData in smallElementSaveDatas)
            {
                var element = new Element_SmallCell(cellSaveData.coord);
                element.cellType = cellSaveData.smallCellType;
                map[element.coord] = element;
            }
            Debug.Log($"[-----System-----] : ElementManager_SamllCell data loaded , SmallCell count <{map.Count}>");
        }

        public void PostInit()
        {
            foreach (var item in map.Values)
            {
                var owners = GetOwnerEdges(item.coord);

                item.cellType = owners != null && owners.Any(x => x != null && x.edgeType != EdgeType.Empty) ? SmallCellType.Wall : SmallCellType.Empty;
            }
        }

        public void Dispose()
        {
            map?.Clear();
        }

        public override Element_SmallCell GetElement(Vector3 worldPosition)
        {
            var coord = CoordUtility.WorldPositionToSmallCoord(worldPosition);
            return GetElement(coord);
        }

        public Element_SmallCell[] GetCellNeighbors(Vector2Int coord)
        {
            var neighbors = new Element_SmallCell[4];
            neighbors[0] = GetLeftCellFromCoord(coord);
            neighbors[1] = GetUpCellFromCoord(coord);
            neighbors[2] = GetRightCellFromCoord(coord);
            neighbors[3] = GetDownCellFromCoord(coord);
            return neighbors;
        }

        public Element_SmallCell GetLeftCellFromCoord(Vector2Int coord)
        {
            return GetElement(coord + DirectionUtility.LEFT);
        }



        public Element_SmallCell GetUpCellFromCoord(Vector2Int coord)
        {
            return GetElement(coord + DirectionUtility.UP);
        }

        public Element_SmallCell GetRightCellFromCoord(Vector2Int coord)
        {
            return GetElement(coord + DirectionUtility.RIGHT);
        }

        public Element_SmallCell GetDownCellFromCoord(Vector2Int coord)
        {
            return GetElement(coord + DirectionUtility.DOWN);
        }

        public Element_Edge[] GetOwnerEdges(Vector2Int coord)
        {
            if (IsEvenNumber(coord.x) && IsEvenNumber(coord.y))
            {
                var checkPosition = CoordUtility.SmallCoordToWorldPosition(coord + DirectionUtility.RIGHTUP);
                var largeCoord = CoordUtility.WorldPositionToLargeCoord(checkPosition);
                var owners = new Element_Edge[4];
                owners[0] = ElementManager_Edge.Instance.GetHorizontal(largeCoord + DirectionUtility.LEFT);
                owners[1] = ElementManager_Edge.Instance.GetVertical(largeCoord);
                owners[2] = ElementManager_Edge.Instance.GetHorizontal(largeCoord);
                owners[3] = ElementManager_Edge.Instance.GetVertical(largeCoord + DirectionUtility.DOWN);
                return owners;
            }
            else if (!IsEvenNumber(coord.x) && IsEvenNumber(coord.y))
            {
                var checkPosition = CoordUtility.SmallCoordToWorldPosition(coord + DirectionUtility.UP);
                var largeCoord = CoordUtility.WorldPositionToLargeCoord(checkPosition);
                var owners = new Element_Edge[1];
                owners[0] = ElementManager_Edge.Instance.GetHorizontal(largeCoord);
                return owners;
            }
            else if (IsEvenNumber(coord.x) && !IsEvenNumber(coord.y))
            {
                var checkPosition = CoordUtility.SmallCoordToWorldPosition(coord + DirectionUtility.RIGHT);
                var largeCoord = CoordUtility.WorldPositionToLargeCoord(checkPosition);
                var owners = new Element_Edge[1];
                owners[0] = ElementManager_Edge.Instance.GetVertical(largeCoord);
                return owners;
            }
            return null;
        }

        private bool IsEvenNumber(int value)
        {
            return value % 2 == 0;
        }

        private void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                foreach (var item in map)
                {
                    item.Value.DrawGizmos();
                }
            }
        }


    }
}
