using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Michsky.MUIP;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

namespace Johnny.SimDungeon
{
    public abstract class SelectableItemViewModel : ViewModelBase
    {
        public ICommand SelectCommand
        {
            get
            {
                return this.m_SelectCommand;
            }
            //set {
            //    this.Set(ref m_SelectCommand, value);
            //}
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

        public Sprite Icon
        {
            get { return this.m_Icon; }
            set { this.Set(ref m_Icon, value); }
        }
        private Sprite m_Icon;

        public Color? BackgroundColor
        {
            get { return this.m_BackgroundColor; }
            set { this.Set(ref m_BackgroundColor, value); }
        }
        private Color? m_BackgroundColor;

        public string Title
        {
            get { return this.m_Title; }
            set { this.Set(ref m_Title, value); }
        }
        private string m_Title;

        public string Description
        {
            get { return this.m_Description; }
            set { this.Set(ref m_Description, value); }
        }
        private string m_Description;


        public SelectableItemViewModel(ICommand selectCommand = null, Color? backgroundColor = null, Sprite icon = null, string title = null, string description = null)
        {
            if (selectCommand != null)
            {
                this.m_SelectCommand = selectCommand;
            }

            if (backgroundColor != null)
            {
                m_BackgroundColor = backgroundColor.Value;
            }

            if (icon != null)
            {
                m_Icon = icon;
            }

            if (title != null)
            {
                m_Title = title;
            }

            if (description != null)
            {
                m_Description = description;
            }

        }

        public void SetCommand(SimpleCommand<SelectableItemViewModel> itemSelectCommand)
        {
            m_SelectCommand = itemSelectCommand;
            RaisePropertyChanged(nameof(SelectCommand));
        }

        public virtual void OnSelectedChanged(bool value)
        { 
        
        }
    }

    public class SelectableItemView : UIView
    {
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_SelectImage;
        [SerializeField] private bool m_BindingIcon;
        [SerializeField] private Image m_Icon;
        [SerializeField] private bool m_BindingBackgroundColor;
        [SerializeField] private Image m_BackgroundImage;
        [SerializeField] private bool m_BindingTitle;
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private bool m_BindingDescription;
        [SerializeField] private TooltipContent m_TooltipContent;

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<SelectableItemView, SelectableItemViewModel>();
            Binding(bindingSet);
            bindingSet.Build();
        }

        protected virtual void Binding(BindingSet<SelectableItemView, SelectableItemViewModel> bindingSet)
        {
            bindingSet.Bind(this.m_Button).For(v => v.onClick).To(vm => vm.SelectCommand).CommandParameter(this.GetDataContext() as SelectableItemViewModel).OneWay();
            bindingSet.Bind(this.m_SelectImage).For(v => v.enabled).To(vm => vm.IsSelected).OneWay();

            if (m_Icon != null && m_BindingIcon)
            {
                bindingSet.Bind(this.m_Icon).For(v => v.sprite).To(vm => vm.Icon).OneWay();
            }
            if (m_BackgroundImage != null && m_BindingBackgroundColor)
            {
                bindingSet.Bind(this.m_BackgroundImage).For(v => v.color).To(vm => vm.BackgroundColor).OneWay();
            }
            if (m_Title != null && m_BindingTitle)
            {
                bindingSet.Bind(this.m_Title).For(v => v.text).To(vm => vm.Title).OneWay();
            }
            if (m_TooltipContent != null && m_BindingDescription)
            {
                bindingSet.Bind(this.m_TooltipContent).For(v => v.description).To(vm => vm.Title).OneWay();
            }
        }

        protected override void OnDestroy()
        {
            this.ClearAllBindings();
        }
    }

}
