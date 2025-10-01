using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Michsky.MUIP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Johnny.SimDungeon
{
    public class SelectableItemViewModel : ViewModelBase
    {
        public ICommand SelectCommand
        {
            get
            {
                return this.m_SelectCommand;
            }
        }
        private ICommand m_SelectCommand;

        public bool IsSelected
        {
            get { return this.m_Selected; }
            set
            {
                this.Set(ref m_Selected, value);
            }
        }
        private bool m_Selected;

        public SelectableItemViewModel(ICommand selectCommand)
        {
            this.m_SelectCommand = selectCommand;
        }
    }

    public abstract class SelectableItemView<VM> : UIView where VM : SelectableItemViewModel
    {
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_Image;

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<SelectableItemView<VM>, VM>();
            Binding(bindingSet);
            bindingSet.Build();
        }

        protected virtual void Binding(BindingSet<SelectableItemView<VM>, VM> bindingSet)
        {
            //bindingSet.Bind(this.gameObject).For(v => v.activeSelf).To(vm => vm.IsSelected).OneWay();
            bindingSet.Bind(this.m_Image).For(v => v.enabled).To(vm => vm.IsSelected).OneWay();
            bindingSet.Bind(this.m_Button).For(v => v.onClick).To(vm => vm.SelectCommand).CommandParameter(this.GetDataContext());
        }
    }

}
