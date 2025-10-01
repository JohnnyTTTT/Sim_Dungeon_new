using SoulGames.EasyGridBuilderPro;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class Entity_Door : Entity_Edge
    {
        public GameObject cutter;
        public GameObject virtualModel;
        public float cutWallSize;
        public float cutWallArch;
        public float cutWalOffsetY;

        public override void UpdateData()
        {
            base.UpdateData();
            edgeElement.SetDoorEntity(this);
        }

        public void SnapToWall()
        {
           var wall = edgeElement.GetWallEntity();


            //var wall = edgeElement.wall;

            //wall.originWalls[0].full = DoCut(wall.originWalls[0].full);
            //wall.originWalls[0].shorten = DoCut(wall.originWalls[0].shorten);

            //wall.originWalls[1].full = DoCut(wall.originWalls[1].full);
            //wall.originWalls[1].shorten = DoCut(wall.originWalls[1].shorten);

            //virtualModel.SetActive(false);
        }
    }
}
