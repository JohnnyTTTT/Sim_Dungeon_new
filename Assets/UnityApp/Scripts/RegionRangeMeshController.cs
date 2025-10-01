using Loxodon.Framework.ObjectPool;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace Johnny.SimDungeon
{
    public class RegionMeshFactory : UnityComponentFactoryBase<RegionRangeMesh>
    {
        private RegionRangeMesh template;
        private Transform parent;

        public RegionMeshFactory(RegionRangeMesh template, Transform parent)
        {
            this.template = template;
            this.parent = parent;
        }

        protected override RegionRangeMesh Create()
        {
            var go = Object.Instantiate(this.template, parent);
            return go.GetComponent<RegionRangeMesh>();
        }

        public override void Reset(RegionRangeMesh obj)
        {
            obj.Dispose();
            obj.gameObject.SetActive(false);
            obj.gameObject.name = "Wait Allocate";
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        public override void Destroy(RegionRangeMesh obj)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(obj);
            }
            else
            {
                Object.DestroyImmediate(obj.gameObject);
            }

        }
    }

    public class RegionRangeMeshController : MonoBehaviour
    {
        [SerializeField] private RegionRangeMesh m_RegionModelPrefab;
        [SerializeField] private Transform m_RegionMeshContainer;

        private IObjectPool<RegionRangeMesh> m_Pool;
        private Dictionary<Region, RegionRangeMesh> regionMeshs = new Dictionary<Region, RegionRangeMesh>();

        public void Init()
        {
            Dispose();

            var factory = new RegionMeshFactory(this.m_RegionModelPrefab, this.m_RegionMeshContainer);
            m_Pool = new ObjectPool<RegionRangeMesh>(factory, 10, 20);

            ElementManager_Region.Instance.OnRegionCreate += OnRegionCreate;
            ElementManager_Region.Instance.OnRegionDestroy += OnRegionDestroy;

            var regions = ElementManager_Region.Instance.regionList;
}

        private void OnRegionCreate(Region region)
        {
            var regionRange = m_Pool.Allocate();
            regionRange.name = region.name;
            regionRange.UpdateRegionMap(region);
            regionMeshs.Add(region, regionRange);
            ShowRegionRange(region, false);
        }

        private void OnRegionDestroy(Region region)
        {
            regionMeshs.Remove(region);
            var freeable = regionMeshs[region].GetComponent<IPooledObject>();
            freeable.Free();
        }

        public void ShowRegionRange(Region region, bool value)
        {
            regionMeshs[region].gameObject.SetActive(value);
        }



        public void Dispose()
        {
            regionMeshs.Clear();
            if (m_Pool != null)
            {
                m_Pool.Dispose();
                m_Pool = null;
            }
            for (int i = m_RegionMeshContainer.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                {
                    Destroy(m_RegionMeshContainer.GetChild(i).gameObject);
                }
                else
                {
                    DestroyImmediate(m_RegionMeshContainer.GetChild(i).gameObject);
                }
            }
        }
    }
}
