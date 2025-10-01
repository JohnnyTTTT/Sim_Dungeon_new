using DungeonArchitect;
using DungeonArchitect.Flow.Domains.Tilemap;
using Loxodon.Framework.Localizations;
using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public enum RoomType
    {
        Undefined,
        OriginaCave,
        EmptyRoom,
        Tavern,
        Hotel,
        HotelRoom,
    }

    public class ElementManager_Region : MonoBehaviour
    {
        public static ElementManager_Region Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<ElementManager_Region>();
                }
                return s_Instance;
            }
        }
        private static ElementManager_Region s_Instance;

        public event Action<Region> OnRegionCreate;
        public event Action<Region> OnRegionDestroy;

        private static int s_RegionID;



        public List<Region> regionList = new List<Region>();
        public Dictionary<Vector2Int, Region> mapForLargeCoord = new Dictionary<Vector2Int, Region>();

        public Dictionary<Vector2Int, Region> mapForSmallCoord = new Dictionary<Vector2Int, Region>();



        [SerializeField] private RegionRangeMeshController m_RegionRangeMeshController;
        public bool drawGizmos;

        [ShowInInspector]
        public int RegionCount
        {
            get
            {
                return regionList.Count;
            }
        }


        public Region CreateRegion(RoomType roomType, HashSet<Element_LargeCell> cells)
        {
            var region = new Region();
            region.Init($"{roomType} - {s_RegionID}", roomType);
            region.AddLargeCells(cells);
            regionList.Add(region);
            s_RegionID++;
            OnRegionCreate?.Invoke(region);
            return region;
        }

        public void RemoveRegion(Region region)
        {
            regionList.Remove(region);
            OnRegionDestroy?.Invoke(region);
        }

        public void RegisterSamallCoord(Vector2Int coord, Region region)
        {
            mapForSmallCoord[coord] = region;
        }

        public void UnregisterSamallCoord(Vector2Int coord, Region region)
        {
            mapForSmallCoord[coord] = null;
        }

        public void RegisterLargeCoord(Vector2Int coord, Region region)
        {
            mapForLargeCoord[coord] = region;
        }

        public void UnregisterLargeCoord(Vector2Int coord, Region region)
        {
            mapForLargeCoord[coord] = null;
        }

        public void ShowRegionRangeFromSmallCoord(Vector2Int position, bool value)
        {
            var region = GetRegionFromSmallCoord(position);
            m_RegionRangeMeshController.ShowRegionRange(region,value);
        }

        public void Init()
        {
            regionList.Clear();
            m_RegionRangeMeshController.Init();
        }

        public void PostInit()
        {
            foreach (var cell in ElementManager_LargeCell.Instance.GetAllElements())
            {
                if (cell.Data.CellType != FlowTilemapCellType.Floor) continue;
                if (mapForLargeCoord.ContainsKey(cell.coord)) continue;

                var regionCells = FloodFill(cell);
                if (regionCells != null && regionCells.Count > 0)
                {
                    var newRegion = CreateRegion(RoomType.EmptyRoom, regionCells);
                    newRegion.CalculateBounds();
                    //Debug.Log($"新区域（ID: {newRegion.name}）被创建，包含 {regionCells.Count} 个格子。区域总数 {Instance.regionList.Count}");
                }
            }

            foreach (var region in regionList)
            {
                CollectSmallCells(region);
            }
            //foreach (var item in ElementManager_SmallCell.Instance.GetAllElements())
            //{
            //    if (item.cellType ==  FlowTilemapSmallCellType.Floor) continue;
            //    if (item.region != null) continue;
            //    var regionCells = FloodFill(cell);
            //}


            foreach (var item in regionList)
            {

            }
        }

        public void Dispose()
        {
            regionList.Clear();
            m_RegionRangeMeshController.Dispose();
        }

        private void OnDestroy()
        {
            regionList.Clear();
        }

        public void CalculateExist(Region region)
        {
            if (region.containedLargeCells == null || region.containedLargeCells.Count == 0)
            {
                RemoveRegion(region);
                Debug.Log($"区域（ID: {name}）被移除。区域总数 {ElementManager_Region.Instance.regionList.Count}");
            }
        }

        private void CollectSmallCells(Region region)
        {
            var position = region.containedLargeCells.First().worldPosition;
            var firstSmall = ElementManager_SmallCell.Instance.GetElement(position);
            var regionCells = FloodFill(firstSmall);
            if (regionCells != null && regionCells.Count > 0)
            {


                foreach (var cell in regionCells)
                {
                    var oldRegion = GetRegionFromSmallCoord(cell.coord);
                    if (oldRegion != null)
                    {
                        oldRegion.RemoveSmallCell(cell);
                    }
                    region.AddSamllCell(cell);
                    cell.cellType = FlowTilemapSmallCellType.Floor;
                }
            }
        }

        public Region GetRegionFromLargeCoord(Vector2Int coord)
        {
            if (mapForLargeCoord.TryGetValue(coord, out var data))
            {
                return data;
            }
            return null;
        }

        public Region GetRegionFromSmallCoord(Vector2Int coord)
        {
            if (mapForSmallCoord.TryGetValue(coord, out var data))
            {
                return data;
            }
            return null;
        }

        public HashSet<Element_SmallCell> FloodFill(Element_SmallCell start, int maxRange = 300)
        {
            var visited = new HashSet<Element_SmallCell>();
            var queue = new Queue<Element_SmallCell>();
            var mapSize = DungeonController.Instance.smallTilemapSize;

            var origin = start.coord;
            int halfRange = maxRange / 2;

            queue.Enqueue(start);
            visited.Add(start);

            var reachedBoundary = false;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var coord = current.coord;

                if (coord.x == 0 || coord.y == 0 || coord.x == mapSize.x - 1 || coord.y == mapSize.y - 1)
                {
                    reachedBoundary = true;
                }

                var neighbors = ElementManager_SmallCell.Instance.GetCellNeighbors(current.coord);
                for (int i = 0; i < 4; i++)
                {
                    var neighbor = neighbors[i];

                    if (neighbor == null || visited.Contains(neighbor)) continue;
                    if (neighbor.cellType == FlowTilemapSmallCellType.Wall) continue;

                    var nCoord = neighbor.coord;
                    if (Mathf.Abs(nCoord.x - origin.x) > halfRange ||
                        Mathf.Abs(nCoord.y - origin.y) > halfRange)
                    {
                        Debug.Log($"FloodFill SmallCell , 超出范围，跳过");
                        continue; // 超出范围，跳过
                    }

                    // 不跳过旧房间格子
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            // 到边界的区域依然返回空
            if (reachedBoundary) return new HashSet<Element_SmallCell>();

            return visited;
        }

        public HashSet<Element_LargeCell> FloodFill(Element_LargeCell start, int maxRange = 150)
        {
            var visited = new HashSet<Element_LargeCell>();
            var queue = new Queue<Element_LargeCell>();
            var mapSize = DungeonController.Instance.smallTilemapSize;

            var origin = start.coord;
            int halfRange = maxRange / 2;

            queue.Enqueue(start);
            visited.Add(start);

            var reachedBoundary = false;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var coord = current.coord;

                if (coord.x == 0 || coord.y == 0 || coord.x == mapSize.x - 1 || coord.y == mapSize.y - 1)
                {
                    reachedBoundary = true;
                }

                var neighbors = ElementManager_LargeCell.Instance.GetCellNeighbors(current.coord);
                for (int i = 0; i < 4; i++)
                {
                    var neighbor = neighbors[i];
                    var dir = DirectionUtility.CardinalDirections[i];

                    if (neighbor == null || visited.Contains(neighbor)) continue;
                    if (DirectionUtility.HasEdgeBetween(current, neighbor, dir)) continue;

                    var nCoord = neighbor.coord;
                    if (Mathf.Abs(nCoord.x - origin.x) > halfRange ||
                        Mathf.Abs(nCoord.y - origin.y) > halfRange)
                    {
                        Debug.Log($"FloodFill LargeCell , 超出范围，跳过");
                        continue; // 超出范围，跳过
                    }

                    // 不跳过旧房间格子
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            // 到边界的区域依然返回空
            if (reachedBoundary) return new HashSet<Element_LargeCell>();

            return visited;
        }

        public void HandleWallsPlacedIncremental(HashSet<Element_LargeCell> allAdjacentCells)
        {
            // 已访问的格子集合，避免重复计算
            var visited = new HashSet<Element_LargeCell>();

            foreach (var cell in allAdjacentCells)
            {
                if (visited.Contains(cell)) continue;

                // 对每个未访问的格子执行 FloodFill
                var regionCells = FloodFill(cell, 150); // 或传入 maxRange
                if (regionCells.Count == 0) continue; // 遇到边界，忽略

                // 标记这些格子已访问
                foreach (var c in regionCells)
                    visited.Add(c);

                var oldRegion = GetRegionFromLargeCoord(regionCells.First().coord);

                if (!regionCells.SetEquals(oldRegion.containedLargeCells))
                {
                    oldRegion.RemoveLargeCells(regionCells);
                    CalculateExist(oldRegion);

                    var newRegion = CreateRegion(RoomType.EmptyRoom, regionCells);
                    newRegion.CalculateBounds();

                    CollectSmallCells(newRegion);
                    Debug.Log($"新区域（ID: {newRegion.name}）被创建，包含 {regionCells.Count} 个格子。区域总数 {Instance.regionList.Count}");
                }
                else
                {
                    Debug.Log($"区域（ID: {oldRegion.name}）未发生改变。区域总数 {Instance.regionList.Count}");
                }
            }
        }


        private void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                foreach (var item in regionList)
                {
                    item.DrawGizmos();
                }
            }

        }
    }
}
