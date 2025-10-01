using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
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
        public DestroyToolViewModel(ICommand selectCommand) :base(selectCommand)
        {
        }

        protected override void OnSelect()
        {
            Debug.Log(11);
        }

    }


    public class DestroyToolItemView : SelectableItemView<DestroyToolViewModel>
    {
        protected override void Start()
        {
            //ViewModel = new DestroyToolViewModel(new SimpleCommand(ViewModel.OnSelect));
            base.Start();
        }

        protected override void Binding(BindingSet<ViewBase<DestroyToolViewModel>, DestroyToolViewModel> bindingSet)
        {
            base.Binding(bindingSet);
        }
    }
}
