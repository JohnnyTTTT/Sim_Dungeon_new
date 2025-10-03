using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Michsky.MUIP;
using SoulGames.EasyGridBuilderPro;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ICommand = Loxodon.Framework.Commands.ICommand;

namespace Johnny.SimDungeon
{
    public class DestroyToolViewModel : SelectableItemViewModel
    {
        public DestroyMode DestroyMode
        {
            get
            {
                return m_DestroyMode;
            }
            set
            {
                Set(ref m_DestroyMode, value);
            }
        }
        private DestroyMode m_DestroyMode;

        public DestroyToolViewModel(DestroyMode destroyMode) : base(null, null, null, null, null)
        {
            m_DestroyMode = destroyMode;
        }

        public override void OnSelectedChanged(bool isSelect)
        {
            if (isSelect)
            {
                Debug.Log(1);
                SpawnManager.Instance.SetDestroyModeInAllGrids(m_DestroyMode);
            }
            else if (SpawnManager.Instance.DestroyMode == DestroyMode)
            {
                Debug.Log(0);
                SpawnManager.Instance.SetDestroyModeInAllGrids(DestroyMode.None);
            }
        }

    }

    public class DestroyToolView : SelectableItemView
    {
        [SerializeField] public DestroyMode m_DestroyMode;

        protected override void Awake()
        {
            var viewModel = new DestroyToolViewModel(m_DestroyMode);
            this.SetDataContext(viewModel);
        }

    }

}
