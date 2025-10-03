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

        public DestroyToolViewModel(ICommand  command):base(command)
        {
        }
        //public DestroyToolViewModel(DestroyMode destroyMode) : base()
        //{
        //    m_DestroyMode = destroyMode;
        //}

        //protected override void OnSelect(bool isSelect)
        //{
        //    //if (isSelect)
        //    //{
        //    //    Debug.Log(m_DestroyMode);
        //    //    SpawnManager.Instance.SetDestroyModeInAllGrids(m_DestroyMode);
        //    //}
        //    //else if (SpawnManager.Instance.DestroyMode == DestroyMode)
        //    //{

        //    //    SpawnManager.Instance.SetDestroyModeInAllGrids(DestroyMode.None);
        //    //}
        //}
    }

    //public class DestroyToolItemView : SelectableItemView<DestroyToolViewModel>
    //{
    //    [SerializeField] private DestroyMode m_DestroyMode;

    //    //public DestroyToolViewModel CreateDataContext()
    //    //{
    //    //    var vm = new DestroyToolViewModel(m_DestroyMode);
    //    //    this.SetDataContext(vm);
    //    //    return (DestroyToolViewModel)this.GetDataContext();
    //    //}
    //    //protected override void Binding(BindingSet<SelectableItemView<DestroyToolViewModel>, DestroyToolViewModel> bindingSet)
    //    //{
    //    //    base.Binding(bindingSet);

    //    //}
    //    public void AA()
    //    { 
        
    //    }

    //}
}
