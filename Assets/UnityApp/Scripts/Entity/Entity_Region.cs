using Sirenix.OdinInspector;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class Entity_Region : MonoBehaviour
    {
        public Region region;

        [ShowInInspector]
        public int LargeCellCount
        {
            get
            {
                return region != null? region.containedLargeCells.Count:0;
            }
        }

        [ShowInInspector]
        public int SmallCellCount
        {
            get
            {
                return region != null ? region.containedSmallCells.Count:0;
            }
        }
    }
}
