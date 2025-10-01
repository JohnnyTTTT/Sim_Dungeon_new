using DungeonArchitect;
using DungeonArchitect.Flow.Domains.Tilemap;
using SoulGames.EasyGridBuilderPro;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }

    public static class DirectionUtility
    {
        public static readonly Vector2Int LEFT = new Vector2Int(-1, 0);
        public static readonly Vector2Int LEFTUP = new Vector2Int(-1, 1);
        public static readonly Vector2Int UP = new Vector2Int(0, 1);
        public static readonly Vector2Int RIGHTUP = new Vector2Int(1, 1);
        public static readonly Vector2Int RIGHT = new Vector2Int(1, 0);
        public static readonly Vector2Int RIGHTDOWN = new Vector2Int(1, -1);
        public static readonly Vector2Int DOWN = new Vector2Int(0, -1);
        public static readonly Vector2Int LEFTDOWN = new Vector2Int(-1, -1);



        public static readonly Vector2Int[] CardinalDirections ={
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1)};

        public static Vector3 dirLeft = Vector3.left;
        public static Vector3 dirUp = Vector3.forward;
        public static Vector3 dirRight = Vector3.right;
        public static Vector3 dirDown = Vector3.back;     
       
        public static Direction ToDirection(FourDirectionalRotation pluginDir)
        {
            return pluginDir switch
            {
                FourDirectionalRotation.North => Direction.Left,   // ��� North(270��) �� �ҵ� Left(270��)
                FourDirectionalRotation.East => Direction.Up,     // ��� East(0��)   �� �ҵ� Up(0��)
                FourDirectionalRotation.South => Direction.Right,  // ��� South(90��) �� �ҵ� Right(90��)
                FourDirectionalRotation.West => Direction.Down,   // ��� West(180��) �� �ҵ� Down(180��)
            };
        }

        public static Direction ToDirection(Quaternion rotation)
        {

            var forward = rotation * Vector3.forward;

            forward.y = 0;
            forward.Normalize();

            var dotUp = Vector3.Dot(forward, dirUp);
            var dotDown = Vector3.Dot(forward, dirDown);
            var dotRight = Vector3.Dot(forward, dirRight);
            var dotLeft = Vector3.Dot(forward, dirLeft);

            float maxDot = Mathf.Max(dotUp, dotDown, dotRight, dotLeft);

            if (maxDot == dotUp)
                return Direction.Up;
            else if (maxDot == dotDown)
                return Direction.Down;
            else if (maxDot == dotRight)
                return Direction.Right;
            else
                return Direction.Left;
        }

        public static FourDirectionalRotation ToEdgeFourDirectionalRotation(Direction myDir)
        {
            return myDir switch
            {
                Direction.Left => FourDirectionalRotation.North,  // �ҵ� Left(270��)�� ��� North(270��)
                Direction.Up => FourDirectionalRotation.East,   // �ҵ� Up(0��)    �� ��� East(0��)
                Direction.Right => FourDirectionalRotation.South,  // �ҵ� Right(90��)�� ��� South(90��)
                Direction.Down => FourDirectionalRotation.West,   // �ҵ� Down(180��)�� ��� West(180��)
            };
        }

        public static FourDirectionalRotation GetFreeFourDirectionalRotationForWorld(Quaternion rotation)
        {
            // ��ȡ��ת��� forward ����ֻ��ˮƽ���Ͽ���
            var forward = rotation * Vector3.forward;
            forward.y = 0;
            forward.Normalize();

            // atan2 �õ��Ƕȣ���Χ -180 ~ 180��
            var angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

            // תΪ 0~360
            if (angle < 0) angle += 360f;

            // ÿ 90�� һ�������������뵽���������
            var index = Mathf.RoundToInt(angle / 90f) % 4;

            return (FourDirectionalRotation)index;
        }

        public static FourDirectionalRotation GetEdgeFourDirectionalRotationForWorld(Quaternion rotation)
        {

            var forward = rotation * Vector3.forward;

            forward.y = 0;
            forward.Normalize();

            var dotUp = Vector3.Dot(forward, dirUp);
            var dotDown = Vector3.Dot(forward, dirDown);
            var dotRight = Vector3.Dot(forward, dirRight);
            var dotLeft = Vector3.Dot(forward, dirLeft);

            float maxDot = Mathf.Max(dotUp, dotDown, dotRight, dotLeft);

            if (maxDot == dotUp)
                return FourDirectionalRotation.East;
            else if (maxDot == dotDown)
                return FourDirectionalRotation.West;
            else if (maxDot == dotRight)
                return FourDirectionalRotation.South;
            else
                return FourDirectionalRotation.North;
        }

        public static Direction GetDirection(Vector3 A, Vector3 B)
        {
            var diff = A - B;
            diff.y = 0;

            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
            {
                return diff.x > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                return diff.z > 0 ? Direction.Up : Direction.Down;
            }
        }

        public static bool HasEdgeBetween(Element_LargeCell a, Element_LargeCell b, Vector2Int dir)
        {
            if (dir == LEFT) 
                return ElementManager_Edge.Instance.GetLeftEdgeFromTileCoord(a.coord).Data.EdgeType > FlowTilemapEdgeType.Empty;
         
            if (dir == UP) 
                return ElementManager_Edge.Instance.GetUpEdgeFromTileCoord(a.coord).Data.EdgeType > FlowTilemapEdgeType.Empty;
          
            if (dir == RIGHT) 
                return ElementManager_Edge.Instance.GetRightEdgeFromTileCoord(a.coord).Data.EdgeType > FlowTilemapEdgeType.Empty;
           
            if (dir == DOWN) 
                return ElementManager_Edge.Instance.GetDownEdgeFromTileCoord(a.coord).Data.EdgeType > FlowTilemapEdgeType.Empty;
            return false;
        }

        public static int GetEdgeConnectedEdgesCount(Element_Edge edge)
        {
            var count = 0;
            var neighbors = ElementManager_Edge.Instance.GetNeighborEdges(edge);
            foreach (var neighbor in neighbors)
            {
                if (neighbor.Data.EdgeType != FlowTilemapEdgeType.Empty)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// ��ȡһ��λ�õĽǵ��������ӱ�
        /// </summary>
        /// <param name="cornerPosition"></param>
        /// <returns></returns>
        public static Element_Edge[] GetCornerConnectedEdges(Vector3 cornerPosition)
        {
            var start = cornerPosition + new Vector3(1f, 0f, 1f);

            var connects = new Element_Edge[4];
            //Left
            connects[0] = ElementManager_Edge.Instance.GetHorizontal(start + Vector3.left);
            //up
            connects[1] = ElementManager_Edge.Instance.GetVertical(start);
            //Right
            connects[2] = ElementManager_Edge.Instance.GetHorizontal(start);
            //Down
            connects[3] = ElementManager_Edge.Instance.GetVertical(start+Vector3.back);
            return connects;
        }

        /// <summary>
        /// �ж�һ��λ�õĽ��Ƿ�����ֱ�Ǳ�
        /// </summary>
        /// <param name="cornerPosition"></param>
        /// <returns></returns>
        public static bool HasCornerConnectRightAngleEdges(Vector3 cornerPosition)
        {
            var edges = GetCornerConnectedEdges(cornerPosition);

            // Left + Up + Down
            if (edges[0] != null && edges[1] != null && edges[3] != null) return true;
            // Up + Left + Right
            if (edges[1] != null && edges[0] != null && edges[2] != null) return true;
            // Right + Up + Down
            if (edges[2] != null && edges[1] != null && edges[3] != null) return true;
            // Down + Left + Right
            if (edges[3] != null && edges[0] != null && edges[2] != null) return true;

            return false;
        }
    }
}
