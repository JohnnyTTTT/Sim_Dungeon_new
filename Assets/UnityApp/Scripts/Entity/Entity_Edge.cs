using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class Entity_Edge : Entity
    {
        public Element_Edge edgeElement;

        protected virtual Vector3 GetOffset()
        {
            return Vector3.zero;
        }


        public override void UpdateData()
        {
            base.UpdateData();

            var worldPosition = buildableObject.GetObjectOriginWorldPosition();

            var front = worldPosition + GetOffset() + transform.forward;
            var back = worldPosition + GetOffset() - transform.forward;

            //Debug.Log(transform.position);
            //Debug.Log(worldPosition);
            var frontCell = ElementManager_LargeCell.Instance.GetElement(front);
            var backCell = ElementManager_LargeCell.Instance.GetElement(back);



            if (Direction == Direction.Up || Direction == Direction.Down)
            {
                var parentElement = front.z > back.z ? frontCell : backCell;
                edgeElement = ElementManager_Edge.Instance.GetDownEdgeFromTileCoord(parentElement.coord);
            }
            else
            {
                var parentElement = front.x > back.x ? frontCell : backCell;

                edgeElement = ElementManager_Edge.Instance.GetLeftEdgeFromTileCoord(parentElement.coord);
            }


        }
    }
}
