using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
    [System.Serializable]
    public class BuildableCategory
    {
        public string name;
        public GridType gridType;
        public List<BuildableObjectSO> buildableObjectSOs;
    }


    public class BuildableAssets : MonoBehaviour
    {
        public static BuildableAssets Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<BuildableAssets>();
                }
                return s_Instance;
            }

        }
        private static BuildableAssets s_Instance;

        public BuildableGenAssets buildableGenAssets;

        private Dictionary<BuildableObjectSO, BuildableGen> m_BuildableGenMap = new Dictionary<BuildableObjectSO, BuildableGen>();

        private void Awake()
        {
            foreach (var item in buildableGenAssets.allAssets)
            {
                if (!m_BuildableGenMap.ContainsKey(item.buildableObjectSO))
                {
                    m_BuildableGenMap.Add(item.buildableObjectSO, item);
                }
            }
        }


        public BuildableGen GetBuildableGen(BuildableObjectSO buildableObjectSO)
        {
            if (m_BuildableGenMap.TryGetValue(buildableObjectSO, out var buildableGen))
            {
                return buildableGen;
            }
            Debug.LogError($"No BuildableGen for : {buildableObjectSO.objectName}");
            return null;
        }
    }
}
