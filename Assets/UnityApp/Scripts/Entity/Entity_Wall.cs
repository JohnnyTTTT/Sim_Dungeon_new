using DungeonArchitect;
using DungeonArchitect.Flow.Domains.Tilemap;
using Sirenix.OdinInspector;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class Entity_Wall : Entity_Edge
    {
        private static int s_Angles = Shader.PropertyToID("_Angles");

        private static int s_OffsetY = Shader.PropertyToID("_OffsetY");
        private static int s_Size = Shader.PropertyToID("_Size");
        private static int s_Arch = Shader.PropertyToID("_Arch");

        public Entity_SubEdge primary;
        public Entity_SubEdge secondary;

        private Material[] m_Materials;

        protected override Vector3 GetOffset()
        {
            return transform.right;
        }

        public override void UpdateData()
        {
            base.UpdateData();
            edgeElement.SetWallEntity(this);

        }

        public void SetCutMaterial(bool value)
        {
            if (Application.isPlaying)
            {
                if (m_Materials == null)
                {
                    m_Materials = new Material[2];
                    m_Materials[0] = primary.GetComponentInChildren<Renderer>().material;
                    m_Materials[1] = secondary.GetComponentInChildren<Renderer>().material;

                    if (Direction == Direction.Up || Direction == Direction.Down)
                    {
                        m_Materials[0].SetFloat(s_Angles, 0f);
                        m_Materials[1].SetFloat(s_Angles, 0f);
                    }
                    else
                    {
                        m_Materials[0].SetFloat(s_Angles, 89.5f);
                        m_Materials[1].SetFloat(s_Angles, 89.5f);
                    }
                }
                foreach (var material in m_Materials)
                {
                    if (value)
                    {
                        var door = edgeElement.GetDoorEntity();
                        material.EnableKeyword("_DOORCUT_ON");
                        material.SetFloat(s_Size, door.cutWallSize);
                        material.SetFloat(s_Arch, door.cutWallArch);
                        material.SetFloat(s_OffsetY, door.cutWalOffsetY);
                    }
                    else
                    {
                        material.DisableKeyword("_DOORCUT_ON");
                    }
                }
            }
        }

        public void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                GizmoUnitily.DrawLine(transform.position + transform.right + new Vector3(0f, 1f, 0f), edgeElement.worldPosition, Color.blue);
            }
        }


    }
}
