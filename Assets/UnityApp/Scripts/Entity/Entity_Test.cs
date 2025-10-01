using SoulGames.EasyGridBuilderPro;
using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class Entity_Test : Entity
    {
        public bool randomRotation;
        public override void UpdateData()
        {
            base.UpdateData();
            var coord = buildableObject.GetObjectOriginCellPosition(out var cellPositionList);

            if (buildableObject is BuildableGridObject buildableGridObject)
            {
            }
        }
    }
}
