using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public static class AtlasUtility
    {
        // 外部凸角 (Row 0 & 4, Col 0 & 4)
        public static readonly Vector2 EX_CORNER_BL = new Vector2(2, 0); // 左下
        public static readonly Vector2 EX_CORNER_BR = new Vector2(0, 0); // 右下
        public static readonly Vector2 EX_CORNER_TL = new Vector2(2, 2); // 左上
        public static readonly Vector2 EX_CORNER_TR = new Vector2(0, 2); // 右上

        // 外部边 (Rows 0, 4, Cols 0, 4 - 中间部分)
        public static readonly Vector2 EX_EDGE_DOWN = new Vector2(1, 2);
        public static readonly Vector2 EX_EDGE_UP = new Vector2(1, 0);
        public static readonly Vector2 EX_EDGE_LEFT = new Vector2(0, 1);
        public static readonly Vector2 EX_EDGE_RIGHT = new Vector2(2, 1);

        // 内部填充 (Row 2, Col 2)
        public static readonly Vector2 INNER_FILL = new Vector2(1, 1);

        // 三面边界 (3-External) - 只有 1 个邻居在内部
        public static readonly Vector2 THREE_EXT_UP = new Vector2(0, 3);    // 内部只有 UP 邻居
        public static readonly Vector2 THREE_EXT_DOWN = new Vector2(3, 2);  // 内部只有 DOWN 邻居
        public static readonly Vector2 THREE_EXT_LEFT = new Vector2(2, 3);  // 内部只有 LEFT 邻居
        public static readonly Vector2 THREE_EXT_RIGHT = new Vector2(0, 3); // 内部只有 RIGHT 邻居

        // 内部凹角 (Internal Concave Corner) - 4 个直角邻居都存在，但对角缺失
        public static readonly Vector2 INNER_CONCAVE_UR = new Vector2(4, 1); // RightUp 缺失
        public static readonly Vector2 INNER_CONCAVE_UL = new Vector2(5, 1); // LeftUp 缺失
        public static readonly Vector2 INNER_CONCAVE_DR = new Vector2(4, 0); // RightDown 缺失
        public static readonly Vector2 INNER_CONCAVE_DL = new Vector2(5, 0); // LeftDown 缺失

        // 孤立 Cell (0 个邻居)
        public static readonly Vector2 ISOLATED = new Vector2(3, 3);
        private const float ATLAS_DIMENSION = 7f;

        // 单元尺寸：1/7
        private const float ATLAS_UNIT = 1f / ATLAS_DIMENSION;

        // 外部获取单元尺寸的方法
        public static float GetAtlasUnitSize()
        {
            return ATLAS_UNIT;
        }

        public static Vector2 GetAtlasUVStart(Vector2 index)
        {
            // 1. 计算 U (列) 起始点
            var u_start = index.x * ATLAS_UNIT;

            // 2. 计算 V (行) 起始点 (需要翻转)
            // V 索引：0-6 (顶部到底部) -> V 坐标：1.0 到 0.0 (底部到顶部)
            // 翻转公式：(总高度 - 1 - 当前行索引) * 单元高度
            var v_start = (ATLAS_DIMENSION - 1 - index.y) * ATLAS_UNIT;

            return new Vector2(u_start, v_start);
        }

        public static Vector2 GetAtlasIndex(Vector2Int coord, HashSet<Vector2Int> regionCells)
        {
            // 检查给定偏移量处的 Cell 是否在 Region 内
            bool IsIn(Vector2Int offset) => regionCells.Contains(coord + offset);

            // 1. 检查 4 个直角邻居 (Cardinal Status)
            var up = IsIn(DirectionUtility.UP);
            var down = IsIn(DirectionUtility.DOWN);
            var left = IsIn(DirectionUtility.LEFT);
            var right = IsIn(DirectionUtility.RIGHT);

            int cardinalCount = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

            // ----------------------------------------------------------------------
            // PART A: 处理基于直角邻居数量的外部拓扑 (0 到 3)
            // ----------------------------------------------------------------------

            // Case 0: 孤立 Cell (0 个直角邻居)
            if (cardinalCount == 0)
            {
                return AtlasUtility.ISOLATED;
            }

            // Case 1: 三面边界 (3-External) (1 个直角邻居在内部)
            if (cardinalCount == 1)
            {
                // 只有某个方向的邻居存在
                if (up) return AtlasUtility.THREE_EXT_UP;
                if (down) return AtlasUtility.THREE_EXT_DOWN;
                if (left) return AtlasUtility.THREE_EXT_LEFT;
                if (right) return AtlasUtility.THREE_EXT_RIGHT;
            }

            // Case 2: 外部凸角 (2 个直角邻居)
            if (cardinalCount == 2)
            {
                // 外部凸角由两个相邻的缺失直角定义
                // 例如：UP 和 RIGHT 缺失 (N=false, E=false) -> 左下凸角
                if (!up && !right) return AtlasUtility.EX_CORNER_BL; // BL: Down/Left are present
                if (!up && !left) return AtlasUtility.EX_CORNER_BR; // BR: Down/Right are present
                if (!down && !right) return AtlasUtility.EX_CORNER_TL; // TL: Up/Left are present
                if (!down && !left) return AtlasUtility.EX_CORNER_TR; // TR: Up/Right are present

                // 注意：如果两个缺失的邻居相对（如 N/S 缺失），则属于简单的 Edge Tile。
                // 为简化，该情况由 Case 3 负责处理。
            }

            // Case 3: 简单外部边 (3 个直角邻居)
            if (cardinalCount == 3)
            {
                // 只有一个方向缺失
                if (!up) return AtlasUtility.EX_EDGE_UP;
                if (!down) return AtlasUtility.EX_EDGE_DOWN;
                if (!left) return AtlasUtility.EX_EDGE_LEFT;
                if (!right) return AtlasUtility.EX_EDGE_RIGHT;
            }

            // ----------------------------------------------------------------------
            // PART B: 处理内部拓扑 (4 个直角邻居)
            // ----------------------------------------------------------------------

            // Case 4: 内部 / 凹角 (4 个直角邻居)
            if (cardinalCount == 4)
            {
                // 检查 4 个对角邻居 (Diagonal Neighbors) - 用于判断内部凹角
                var NW_Missing = !IsIn(DirectionUtility.LEFTUP);
                var NE_Missing = !IsIn(DirectionUtility.RIGHTUP);
                var SW_Missing = !IsIn(DirectionUtility.LEFTDOWN);
                var SE_Missing = !IsIn(DirectionUtility.RIGHTDOWN);

                // 内部凹角 (Internal Concave Corner)
                // 任何一个对角邻居缺失，都表明此 Cell 的该角需要绘制内圆弧。
                if (NW_Missing) return AtlasUtility.INNER_CONCAVE_UL; // Left-Up 缺失
                if (NE_Missing) return AtlasUtility.INNER_CONCAVE_UR; // Right-Up 缺失
                if (SW_Missing) return AtlasUtility.INNER_CONCAVE_DL; // Left-Down 缺失
                if (SE_Missing) return AtlasUtility.INNER_CONCAVE_DR; // Right-Down 缺失

                // 内部填充 (Inner Fill)
                return AtlasUtility.INNER_FILL;
            }

            // Fallback: 不应该发生
            return AtlasUtility.INNER_FILL;
        }



    }
}
