using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class RegionEntityManager : MonoBehaviour
    {
        [SerializeField] private Entity_Region prefab;

        private List<Entity_Region> m_RegionEntities = new List<Entity_Region>();

        private void Start()
        {
            ElementManager_Region.Instance.OnRegionCreate += OnRegionCreate;
        }

        private void OnRegionCreate(Region region)
        {
            var entity = Instantiate(prefab, region.center, Quaternion.identity, this.transform);
            entity.region = region;
            entity.name = region.name;
            m_RegionEntities.Add(entity);
        }

        private void OnDestroy()
        {
            for (int i = m_RegionEntities.Count - 1; i >= 0; i--)
            {
                Destroy(m_RegionEntities[i].gameObject);
            }
        }
    }
}
