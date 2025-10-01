using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public static class AtlasUtility
    {
        // �ⲿ͹�� (Row 0 & 4, Col 0 & 4)
        public static readonly Vector2 EX_CORNER_BL = new Vector2(2, 0); // ����
        public static readonly Vector2 EX_CORNER_BR = new Vector2(0, 0); // ����
        public static readonly Vector2 EX_CORNER_TL = new Vector2(2, 2); // ����
        public static readonly Vector2 EX_CORNER_TR = new Vector2(0, 2); // ����

        // �ⲿ�� (Rows 0, 4, Cols 0, 4 - �м䲿��)
        public static readonly Vector2 EX_EDGE_DOWN = new Vector2(1, 2);
        public static readonly Vector2 EX_EDGE_UP = new Vector2(1, 0);
        public static readonly Vector2 EX_EDGE_LEFT = new Vector2(0, 1);
        public static readonly Vector2 EX_EDGE_RIGHT = new Vector2(2, 1);

        // �ڲ���� (Row 2, Col 2)
        public static readonly Vector2 INNER_FILL = new Vector2(1, 1);

        // ����߽� (3-External) - ֻ�� 1 ���ھ����ڲ�
        public static readonly Vector2 THREE_EXT_UP = new Vector2(0, 3);    // �ڲ�ֻ�� UP �ھ�
        public static readonly Vector2 THREE_EXT_DOWN = new Vector2(3, 2);  // �ڲ�ֻ�� DOWN �ھ�
        public static readonly Vector2 THREE_EXT_LEFT = new Vector2(2, 3);  // �ڲ�ֻ�� LEFT �ھ�
        public static readonly Vector2 THREE_EXT_RIGHT = new Vector2(0, 3); // �ڲ�ֻ�� RIGHT �ھ�

        // �ڲ����� (Internal Concave Corner) - 4 ��ֱ���ھӶ����ڣ����Խ�ȱʧ
        public static readonly Vector2 INNER_CONCAVE_UR = new Vector2(4, 1); // RightUp ȱʧ
        public static readonly Vector2 INNER_CONCAVE_UL = new Vector2(5, 1); // LeftUp ȱʧ
        public static readonly Vector2 INNER_CONCAVE_DR = new Vector2(4, 0); // RightDown ȱʧ
        public static readonly Vector2 INNER_CONCAVE_DL = new Vector2(5, 0); // LeftDown ȱʧ

        // ���� Cell (0 ���ھ�)
        public static readonly Vector2 ISOLATED = new Vector2(3, 3);
        private const float ATLAS_DIMENSION = 7f;

        // ��Ԫ�ߴ磺1/7
        private const float ATLAS_UNIT = 1f / ATLAS_DIMENSION;

        // �ⲿ��ȡ��Ԫ�ߴ�ķ���
        public static float GetAtlasUnitSize()
        {
            return ATLAS_UNIT;
        }

        public static Vector2 GetAtlasUVStart(Vector2 index)
        {
            // 1. ���� U (��) ��ʼ��
            var u_start = index.x * ATLAS_UNIT;

            // 2. ���� V (��) ��ʼ�� (��Ҫ��ת)
            // V ������0-6 (�������ײ�) -> V ���꣺1.0 �� 0.0 (�ײ�������)
            // ��ת��ʽ��(�ܸ߶� - 1 - ��ǰ������) * ��Ԫ�߶�
            var v_start = (ATLAS_DIMENSION - 1 - index.y) * ATLAS_UNIT;

            return new Vector2(u_start, v_start);
        }

        public static Vector2 GetAtlasIndex(Vector2Int coord, HashSet<Vector2Int> regionCells)
        {
            // ������ƫ�������� Cell �Ƿ��� Region ��
            bool IsIn(Vector2Int offset) => regionCells.Contains(coord + offset);

            // 1. ��� 4 ��ֱ���ھ� (Cardinal Status)
            var up = IsIn(DirectionUtility.UP);
            var down = IsIn(DirectionUtility.DOWN);
            var left = IsIn(DirectionUtility.LEFT);
            var right = IsIn(DirectionUtility.RIGHT);

            int cardinalCount = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

            // ----------------------------------------------------------------------
            // PART A: �������ֱ���ھ��������ⲿ���� (0 �� 3)
            // ----------------------------------------------------------------------

            // Case 0: ���� Cell (0 ��ֱ���ھ�)
            if (cardinalCount == 0)
            {
                return AtlasUtility.ISOLATED;
            }

            // Case 1: ����߽� (3-External) (1 ��ֱ���ھ����ڲ�)
            if (cardinalCount == 1)
            {
                // ֻ��ĳ��������ھӴ���
                if (up) return AtlasUtility.THREE_EXT_UP;
                if (down) return AtlasUtility.THREE_EXT_DOWN;
                if (left) return AtlasUtility.THREE_EXT_LEFT;
                if (right) return AtlasUtility.THREE_EXT_RIGHT;
            }

            // Case 2: �ⲿ͹�� (2 ��ֱ���ھ�)
            if (cardinalCount == 2)
            {
                // �ⲿ͹�����������ڵ�ȱʧֱ�Ƕ���
                // ���磺UP �� RIGHT ȱʧ (N=false, E=false) -> ����͹��
                if (!up && !right) return AtlasUtility.EX_CORNER_BL; // BL: Down/Left are present
                if (!up && !left) return AtlasUtility.EX_CORNER_BR; // BR: Down/Right are present
                if (!down && !right) return AtlasUtility.EX_CORNER_TL; // TL: Up/Left are present
                if (!down && !left) return AtlasUtility.EX_CORNER_TR; // TR: Up/Right are present

                // ע�⣺�������ȱʧ���ھ���ԣ��� N/S ȱʧ���������ڼ򵥵� Edge Tile��
                // Ϊ�򻯣�������� Case 3 ������
            }

            // Case 3: ���ⲿ�� (3 ��ֱ���ھ�)
            if (cardinalCount == 3)
            {
                // ֻ��һ������ȱʧ
                if (!up) return AtlasUtility.EX_EDGE_UP;
                if (!down) return AtlasUtility.EX_EDGE_DOWN;
                if (!left) return AtlasUtility.EX_EDGE_LEFT;
                if (!right) return AtlasUtility.EX_EDGE_RIGHT;
            }

            // ----------------------------------------------------------------------
            // PART B: �����ڲ����� (4 ��ֱ���ھ�)
            // ----------------------------------------------------------------------

            // Case 4: �ڲ� / ���� (4 ��ֱ���ھ�)
            if (cardinalCount == 4)
            {
                // ��� 4 ���Խ��ھ� (Diagonal Neighbors) - �����ж��ڲ�����
                var NW_Missing = !IsIn(DirectionUtility.LEFTUP);
                var NE_Missing = !IsIn(DirectionUtility.RIGHTUP);
                var SW_Missing = !IsIn(DirectionUtility.LEFTDOWN);
                var SE_Missing = !IsIn(DirectionUtility.RIGHTDOWN);

                // �ڲ����� (Internal Concave Corner)
                // �κ�һ���Խ��ھ�ȱʧ���������� Cell �ĸý���Ҫ������Բ����
                if (NW_Missing) return AtlasUtility.INNER_CONCAVE_UL; // Left-Up ȱʧ
                if (NE_Missing) return AtlasUtility.INNER_CONCAVE_UR; // Right-Up ȱʧ
                if (SW_Missing) return AtlasUtility.INNER_CONCAVE_DL; // Left-Down ȱʧ
                if (SE_Missing) return AtlasUtility.INNER_CONCAVE_DR; // Right-Down ȱʧ

                // �ڲ���� (Inner Fill)
                return AtlasUtility.INNER_FILL;
            }

            // Fallback: ��Ӧ�÷���
            return AtlasUtility.INNER_FILL;
        }



    }
}
