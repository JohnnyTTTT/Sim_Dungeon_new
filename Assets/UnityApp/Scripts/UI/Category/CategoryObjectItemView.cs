using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Michsky.MUIP;
using SoulGames.EasyGridBuilderPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Johnny.SimDungeon
{
    public class CategoryObjectItemViewModel : SelectableItemViewModel
    {
        public BuildableObjectUICategorySO Data
        {
            get { return this.data; }
            set { this.Set(ref data, value); }
        }
        private BuildableObjectUICategorySO data;

        public Sprite Icon
        {
            get { return this.icon; }
            set { this.Set(ref icon, value); }
        }
        private Sprite icon;

        public string Description
        {
            get { return this.m_Description; }
            set { this.Set(ref m_Description, value); }
        }
        private string m_Description;

        public CategoryObjectItemViewModel(BuildableObjectUICategorySO categorySO,Loxodon.Framework.Commands.ICommand selectCommand) :
            base(selectCommand)
        {
            if (categorySO != null)
            {
                Icon = categorySO.categoryIcon;
                Data = categorySO;
                Description = categorySO.categoryName;
            }
        }
    }
    public class CategoryObjectItemView : SelectableItemView<CategoryObjectItemViewModel>
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TooltipContent m_TooltipContent;
        protected override void Binding(BindingSet<ViewBase<CategoryObjectItemViewModel>, CategoryObjectItemViewModel> bindingSet)
        {
            base.Binding(bindingSet);
            bindingSet.Bind(m_Icon).For(v => v.sprite).To(vm => vm.Icon).OneWay();
            bindingSet.Bind(m_TooltipContent).For(v => v.description).To(vm => vm.Description).OneWay();
        }

    }
}
