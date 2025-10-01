using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.ViewModels;
using Michsky.MUIP;
using SoulGames.EasyGridBuilderPro;
using System.Windows.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Johnny.SimDungeon
{
    public class BuildableGenItemViewModel : SelectableItemViewModel
    {
        public BuildableGen Data
        {
            get { return this.data; }
            set { this.Set(ref data, value); }
        }
        private BuildableGen data;

        public Sprite Icon
        {
            get { return this.icon; }
            set { this.Set(ref icon, value); }
        }
        private Sprite icon;

        public string Title
        {
            get { return this.m_Title; }
            set { this.Set(ref m_Title, value); }
        }
        private string m_Title;

        public bool Active
        {
            get { return this.active; }
            set { this.Set(ref active, value); }
        }
        private bool active;

        public string Description
        {
            get { return this.m_Description; }
            set { this.Set(ref m_Description, value); }
        }
        private string m_Description;


        public BuildableGenItemViewModel(BuildableGen buildableGen, Loxodon.Framework.Commands.ICommand selectCommand) :
            base(selectCommand)
        {
            Data = buildableGen;
            Icon = buildableGen.buildableObjectSO.objectIcon;
            Title = buildableGen.buildableObjectSO.objectName;
            Description = Data.description;
        }
    }

    public class BuildableObjectItemView : SelectableItemView<BuildableGenItemViewModel>
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TooltipContent m_TooltipContent;

        protected override void Binding(BindingSet<SelectableItemView<BuildableGenItemViewModel>, BuildableGenItemViewModel> bindingSet)
        {
            base.Binding(bindingSet);
            bindingSet.Bind(m_Icon).For(v => v.sprite).To(vm => vm.Icon).OneWay();
            bindingSet.Bind(m_TooltipContent).For(v => v.description).To(vm => vm.Description).OneWay();
        }
    }
}
