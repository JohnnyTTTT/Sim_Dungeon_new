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
}
