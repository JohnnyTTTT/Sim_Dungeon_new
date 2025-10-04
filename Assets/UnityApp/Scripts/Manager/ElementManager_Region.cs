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
        public event Action<Region> OnRegionRemove;

        private static int s_RegionID;

        public List<Region> regionList = new List<Region>();
        public Dictionary<Vector2Int, Region> mapForLargeCoord = new Dictionary<Vector2Int, Region>();
        public Dictionary<Vector2Int, Region> mapForSmallCoord = new Dictionary<Vector2Int, Region>();

        public bool drawGizmos;

        [ShowInInspector]
        public int RegionCount
        {
            get
            {
                return regionList.Count;
            }
        }


        public Region CreateRegion(RoomType roomType, HashSet<Vector2Int> cells)
        {
            var region = new Region();
            region.Init($"{roomType} - {s_RegionID}", roomType);
            region.AddLargeCells(cells);
            CollectSmallCells(region);
            regionList.Add(region);
            s_RegionID++;
            region.CalculateBounds();
            OnRegionCreate?.Invoke(region);
            return region;
        }

        public void RemoveRegion(Region region)
        {
            regionList.Remove(region);
            OnRegionRemove?.Invoke(region);
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



        public void Initialize()
        {
            regionList.Clear();
        }

        public void PostInit()
        {
            foreach (var cell in ElementManager_LargeCell.Instance.GetAllElements())
            {
                if (cell.cellType != LargelCellType.Floor) continue;
                if (mapForLargeCoord.ContainsKey(cell.coord)) continue;

                var regionCells = FloodFillLargeCoord(cell.coord);
                if (regionCells != null && regionCells.Count > 0)
                {
                    var newRegion = CreateRegion(RoomType.EmptyRoom, regionCells);

                    //Debug.Log($"新区域（ID: {newRegion.name}）被创建，包含 {regionCells.Count} 个格子。区域总数 {Instance.regionList.Count}");
                }
            }
            Debug.Log($"[-----System-----] : ElementManager_Region postInit , Region count <{regionList.Count}>");
        }

        public void LoadElements(List<RegionSaveData> regionSaveDatas)
        {
            foreach (var data in regionSaveDatas)
            {
                var newRegion = CreateRegion(data.roomType, data.containedLargeCells.ToHashSet());
            }
            Debug.Log($"[-----System-----] : ElementManager_Region data loaded , Region count <{regionList.Count}>");
        }

        public void Dispose()
        {
            regionList?.Clear();
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
            var worldPosition = CoordUtility.LargeCoordToWorldPosition(region.containedLargeCells.First());
            var firstSmall = CoordUtility.WorldPositionToSmallCoord(worldPosition);
            var regionCells = FloodFillSmallCoord(firstSmall);
            if (regionCells != null && regionCells.Count > 0)
            {
                foreach (var coord in regionCells)
                {
                    var oldRegion = GetRegionFromSmallCoord(coord);
                    if (oldRegion != null)
                    {
                        oldRegion.RemoveSmallCell(coord);
                    }
                    region.AddSamllCell(coord);
                    var cell = ElementManager_SmallCell.Instance.GetElement(coord);
                    cell.cellType = SmallCellType.Floor;
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

        public HashSet<Vector2Int> FloodFillSmallCoord(Vector2Int start, int maxRange = 300)
        {
            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();
            var mapSize = WorldManager.Instance.smallTilemapSize;

            var origin = start;
            int halfRange = maxRange / 2;

            queue.Enqueue(start);
            visited.Add(start);

            var reachedBoundary = false;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var coord = current;

                if (coord.x == 0 || coord.y == 0 || coord.x == mapSize.x - 1 || coord.y == mapSize.y - 1)
                {
                    reachedBoundary = true;
                }

                var neighbors = ElementManager_SmallCell.Instance.GetCellNeighbors(current);
                for (int i = 0; i < 4; i++)
                {
                    var neighbor = neighbors[i];

                    if (neighbor == null || visited.Contains(neighbor.coord)) continue;
                    if (neighbor.cellType == SmallCellType.Wall) continue;

                    var nCoord = neighbor.coord;
                    if (Mathf.Abs(nCoord.x - origin.x) > halfRange ||
                        Mathf.Abs(nCoord.y - origin.y) > halfRange)
                    {
                        Debug.Log($"FloodFill SmallCell , 超出范围，跳过");
                        continue; // 超出范围，跳过
                    }

                    // 不跳过旧房间格子
                    visited.Add(neighbor.coord);
                    queue.Enqueue(neighbor.coord);
                }
            }

            // 到边界的区域依然返回空
            if (reachedBoundary) return new HashSet<Vector2Int>();

            return visited;
        }

        public HashSet<Vector2Int> FloodFillLargeCoord(Vector2Int start, int maxRange = 150)
        {
            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();
            var mapSize = WorldManager.Instance.smallTilemapSize;

            var origin = start;
            int halfRange = maxRange / 2;

            queue.Enqueue(start);
            visited.Add(start);

            var reachedBoundary = false;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var coord = current;

                if (coord.x == 0 || coord.y == 0 || coord.x == mapSize.x - 1 || coord.y == mapSize.y - 1)
                {
                    reachedBoundary = true;
                }

                var neighbors = ElementManager_LargeCell.Instance.GetCellNeighbors(current);
                for (int i = 0; i < 4; i++)
                {
                    var neighbor = neighbors[i];
                    var dir = DirectionUtility.CardinalDirections[i];

                    if (neighbor == null || visited.Contains(neighbor.coord)) continue;
                    if (DirectionUtility.HasEdgeBetween(current, neighbor.coord, dir)) continue;

                    var nCoord = neighbor.coord;
                    if (Mathf.Abs(nCoord.x - origin.x) > halfRange || Mathf.Abs(nCoord.y - origin.y) > halfRange)
                    {
                        Debug.Log($"FloodFill LargeCell , 超出范围，跳过");
                        continue; // 超出范围，跳过
                    }

                    // 不跳过旧房间格子
                    visited.Add(neighbor.coord);
                    queue.Enqueue(neighbor.coord);
                }
            }

            // 到边界的区域依然返回空
            if (reachedBoundary) return new HashSet<Vector2Int>();

            return visited;
        }

        public void HandleWallsPlacedIncremental(HashSet<Vector2Int> allAdjacentCells)
        {
            // 已访问的格子集合，避免重复计算
            var visited = new HashSet<Vector2Int>();

            foreach (var cell in allAdjacentCells)
            {
                if (visited.Contains(cell)) continue;

                // 对每个未访问的格子执行 FloodFill
                var regionCells = FloodFillLargeCoord(cell, 150); // 或传入 maxRange

                if (regionCells.Count == 0) continue; // 遇到边界，忽略

                // 标记这些格子已访问
                foreach (var c in regionCells)
                {
                    visited.Add(c);
                }
                var oldRegion = GetRegionFromLargeCoord(regionCells.First());

                if (!regionCells.SetEquals(oldRegion.containedLargeCells))
                {
                    oldRegion.RemoveLargeCells(regionCells);
                    oldRegion.CalculateBounds();
                    CalculateExist(oldRegion);

                    var newRegion = CreateRegion(RoomType.EmptyRoom, regionCells);

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
