using System;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class OutlineManager : MonoBehaviour
    {
        private Entity m_LastHoverEntity;
        private Entity m_LastSelectEntity;

        private void Start()
        {
            SelectionManager.Instance.OnEntityHover += OnEntityHover;
            SelectionManager.Instance.OnEntitySelected += OnEntitySelected;
        }

        private void OnEntityHover(Entity entity)
        {
            if (entity != null)
            {
                entity.OutlineVisible(true);
                m_LastHoverEntity = entity;
            }
            else if(m_LastHoverEntity != null)
            {
                m_LastHoverEntity.OutlineVisible(false);
            }
        }

        private void OnEntitySelected(Entity entity)
        {
            if (entity != null)
            {
                entity.OutlineVisible(true);
                m_LastSelectEntity = entity;
            }
            else if (m_LastSelectEntity != null)
            {
                m_LastSelectEntity.OutlineVisible(false);
            }
        }
    }
}
