using DungeonArchitect;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class PathStartConstraints : ScriptableObject, IGridFlowLayoutNodePositionConstraint
    {
        public bool CanCreateNodeAt(int currentPathPosition, int totalPathLength, Vector2Int nodeCoord, Vector2Int gridSize)
        {
            Debug.Log(gridSize);
            return nodeCoord.x == 1 && nodeCoord.y == 1;

        }
    }
}
