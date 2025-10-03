using System;
using Unity.AI.Navigation;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class NavMeshController : MonoBehaviour
    {
        public NavMeshSurface m_GroundNavMeshSurface;
        private void Start()
        {
            WorldManager.OnWorldCreated += OnWorldCreated;
            //m_GroundNavMeshSurface.BuildNavMesh();
        }

        private void OnWorldCreated()
        {
            m_GroundNavMeshSurface.BuildNavMesh();
        }
    }
}
