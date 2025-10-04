using DungeonArchitect;
using System;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public static class CoordUtility
    {
        private static Vector3 SmllGridOriginPosition = new Vector3(-0.5f, 0f, -0.5f);

        public static IntVector2 ToIntVector2(this Vector2Int coord)
        {
            return new IntVector2(coord.x, coord.y);
        }
        public static Vector2Int ToVector2Int(this IntVector2 coord)
        {
            return new Vector2Int(coord.x, coord.y);
        }

        //Small
        public static Vector3 SmallCoordToWorldPosition(Vector2Int coord)
        {
            if (Application.isPlaying)
            {
                return SpawnManager.Instance.m_EasyGridBuilderPro_SmallCell.GetCellWorldPosition(coord, 0) + new Vector3(0.5f, 0f, 0.5f);
            }
            else
            {
                return new Vector3(coord.x, 0, coord.y) + SmllGridOriginPosition;
            }
        }

        public static Vector2Int WorldPositionToSmallCoord(Vector3 worldPosition)
        {
            if (Application.isPlaying)
            {
                return SpawnManager.Instance.m_EasyGridBuilderPro_SmallCell.GetActiveGridCellPosition(worldPosition);
            }
            else
            {
                //Debug.Log(new Vector2Int(
                //    Mathf.FloorToInt((worldPosition - Size1GridOriginPosition).x),
                //    Mathf.FloorToInt((worldPosition - Size1GridOriginPosition).z)));
                return new Vector2Int(
                    Mathf.FloorToInt((worldPosition - SmllGridOriginPosition).x),
                    Mathf.FloorToInt((worldPosition - SmllGridOriginPosition).z));
            }
        }

        public static bool IsSamllCoordInBounds(Vector2Int cellPosition)
        {
            return SpawnManager.Instance.m_EasyGridBuilderPro_SmallCell.IsWithinActiveGridBounds(cellPosition);
        }


        //Large
        public static Vector3 LargeCoordToWorldPosition(Vector2Int coord)
        {
            return DungeonManager.Instance.groundDungeon.TileCoordToWorldCoord(coord.ToIntVector2());
        }

        public static Vector3 EdgeCoordToWorldPosition(Vector2Int coord,bool horizontalEdge)
        {
            if (horizontalEdge)
            {
               return LargeCoordToWorldPosition(coord)+ Vector3.back;
            }
            else
            {
                return LargeCoordToWorldPosition(coord) + Vector3.left;
            }
        }

        public static Vector2Int WorldPositionToLargeCoord(Vector3 coord)
        {
            return DungeonManager.Instance.groundDungeon.WorldPositionToTilemapCoord(coord).ToVector2Int();
        }

        public static bool IsLargeCoordInBounds(Vector2Int cellPosition)
        {
            var x = cellPosition.x;
            var z = cellPosition.y;
            //var gridWidth = DungeonController.Instance.dungeonModel.Tilemap.Width;
            //var gridLength = DungeonController.Instance.dungeonModel.Tilemap.Height;
            return x >= 0 && z >= 0 && x < 80 && z < 80;
        }

    }
}
