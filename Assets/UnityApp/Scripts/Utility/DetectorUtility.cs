//using DungeonArchitect;
//using DungeonArchitect.Flow.Domains.Tilemap;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Johnny.SimDungeon
//{
//    public class DetectorUtility
//    {
//        // ��ˮ����㷨�����������·��䣬Ҳ���Բ�־ɷ���
//        public static HashSet<Element_Cell> FloodFill(Element_Cell start)
//        {
//            var visited = new HashSet<Element_Cell>();
//            var queue = new Queue<Element_Cell>();
//            var mapSize = DungeonController.Instance.tilemapSize;

//            queue.Enqueue(start);
//            visited.Add(start);

//            var reachedBoundary = false;

//            while (queue.Count > 0)
//            {
//                var current = queue.Dequeue();
//                var coord = current.Data.TileCoord;

//                // ����Ƿ񵽴��ͼ�߽�
//                if (coord.x == 0 || coord.y == 0 || coord.x == mapSize.x - 1 || coord.y == mapSize.y - 1)
//                {
//                    reachedBoundary = true;
//                }

//                for (int i = 0; i < 4; i++)
//                {
//                    var neighbor = current.neighbors[i];
//                    var dir = DirectionUtility.CardinalDirections[i];

//                    if (neighbor == null || visited.Contains(neighbor)) continue;
//                    if (HasWallBetween(current, neighbor, dir)) continue;

//                    visited.Add(neighbor);
//                    queue.Enqueue(neighbor);
//                }
//            }

//            // �������߽磬˵������δ���
//            if (reachedBoundary)
//            {
//                return new HashSet<Element_Cell>();
//            }

//            return visited;
//        }


//        // ������ǽʱ����������ʽ�����⣨֧�ֲ�־ɷ��䣩
//        public static void HandleWallPlacedIncremental(Entity_Edge entity)
//        {
//            if (ElementManager_Edge.Instance.CountConnectedEdges(entity.edgeElement) < 2)
//            {
//                Debug.Log("��ǽ��������2�����γɷ�տռ䡣���������顣");
//                return;
//            }

//            var cellA = entity.edgeElement.primaryCell;
//            var cellB = entity.edgeElement.secondaryCell;

//            if (cellA == null || cellB == null) return;

//            // ========== ����1: �ռ���Ӱ�� cell ==========
//            var affectedCells = new HashSet<Element_Cell> { cellA, cellB };

//            // �ռ���Ӱ��ľɷ��� cell���ھ����ھɷ��䣩
//            foreach (var c in affectedCells.ToList())
//            {
//                for (int i = 0; i < 4; i++)
//                {
//                    var neighbor = c.neighbors[i];
//                    if (neighbor != null)
//                        affectedCells.Add(neighbor);
//                }
//            }

//            // ========== ����2: FloodFill �������� ==========
//            var processed = new HashSet<Element_Cell>();
//            foreach (var c in affectedCells)
//            {
//                if (processed.Contains(c)) continue;

//                var newRegionCells = DetectorUtility.FloodFill(c);
//                if (newRegionCells.Count == 0) continue; // ��Ч��������

//                // �Ӿɷ������Ƴ���Щ cell
//                foreach (var cell in newRegionCells)
//                {
//                    if (cell.region != null)
//                    {
//                        cell.region.RemoveCell(cell);
//                    }
//                }

//                // �����·���
//                var newRoom = ElementManager_Region.Instance.CreateRegion(RoomType.EmptyRoom);
//                newRoom.AddCells(newRegionCells);
//                //Debug.Log($"�·��䣨ID: {newRoom.name}�������������� {newRegionCells.Count} �����ӡ��������� {ElementManager_Region.Instance.regionList.Count}");

//                processed.UnionWith(newRegionCells);
//            }

//        }

//        // ��������������������ڵ�Ԫ��֮���Ƿ���ǽ
//        private static bool HasWallBetween(Element_Cell a, Element_Cell b, IntVector2 dir)
//        {
//            if (dir == DirectionUtility.UP) return a.upEdge.Data.EdgeType > FlowTilemapEdgeType.Empty;
//            if (dir == DirectionUtility.DOWN) return a.downEdge.Data.EdgeType > FlowTilemapEdgeType.Empty;
//            if (dir == DirectionUtility.LEFT) return a.leftEdge.Data.EdgeType > FlowTilemapEdgeType.Empty;
//            if (dir == DirectionUtility.RIGHT) return a.rightEdge.Data.EdgeType > FlowTilemapEdgeType.Empty;
//            return false;
//        }
//    }
//}
