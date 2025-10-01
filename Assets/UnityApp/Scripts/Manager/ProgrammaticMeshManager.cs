using DungeonArchitect;
using DungeonArchitect.Flow.Domains.Tilemap;
using Loxodon.Framework.Examples;
using Loxodon.Framework.ObjectPool;
using SharpNav;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
   

    public class ProgrammaticMeshManager : MonoBehaviour
    {
        public static ProgrammaticMeshManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<ProgrammaticMeshManager>();
                }
                return s_Instance;
            }

        }
        private static ProgrammaticMeshManager s_Instance;

        [SerializeField] private InvalidAreaMesh m_InvalidAreaMesh;

        public void Init()
        {
            m_InvalidAreaMesh.UpdateMesh();
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            if (m_InvalidAreaMesh != null)
            {
                m_InvalidAreaMesh.Clear();
            }
        }


    }
}
