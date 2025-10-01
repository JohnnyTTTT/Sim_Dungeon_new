using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.ViewModels;
using Michsky.MUIP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Johnny.SimDungeon
{
    public class ListButtonView<VM> : ViewBase<VM> where VM : SelectableItemViewModel
    {
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_Image;
  
    
        protected override void Binding(BindingSet<ViewBase<VM>, VM> bindingSet)
        {
            bindingSet.Bind(this.m_Image).For(v => v.enabled).To(vm => vm.IsSelected).OneWay();
            bindingSet.Bind(this.m_Button).For(v => v.onClick).To(vm => vm.SelectCommand).CommandParameter(this.ViewModel);
        }
    }

}
