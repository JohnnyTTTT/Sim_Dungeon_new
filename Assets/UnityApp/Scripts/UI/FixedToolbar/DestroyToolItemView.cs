using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Messaging;
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
        protected SimpleCommand ItemSelectCommand;


        public DestroyToolViewModel() : base()
        {
            ItemSelectCommand = new SimpleCommand(OnItemSelect);
            SetSelectCommand(ItemSelectCommand);
        }



        private void OnItemSelect()
        {
            IsSelected = true;
            SpawnManager.Instance.SetDestroyModeInAllGrids(DestroyMode.Edge);
        }
    }


    public class DestroyToolItemView : SelectableItemView<DestroyToolViewModel>
    {
        protected override void Awake()
        {
            var vm = new DestroyToolViewModel();
            this.SetDataContext(vm);
        }
    }
}
