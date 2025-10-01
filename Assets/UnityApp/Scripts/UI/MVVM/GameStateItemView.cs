using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Michsky.MUIP;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Johnny.SimDungeon
{
    public class GameStateItemViewModel : SelectableItemViewModel
    {
        private IDisposable m_Subscription;

        public GameState GameState
        {
            get
            {
                return m_GameState;
            }
            set
            {
                Set(ref m_GameState, value);
            }
        }
        private GameState m_GameState;

        public Sprite Icon
        {
            get { return this.m_Icon; }
            set { this.Set(ref m_Icon, value); }
        }
        private Sprite m_Icon;


        public GameStateItemViewModel(GameState gameState, Sprite icon, ICommand selectCommand) : base(selectCommand)
        {
            m_GameState = gameState;
            m_Icon = icon;
        }
    }

    public class GameStateItemView : SelectableItemView<GameStateItemViewModel>
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TooltipContent m_TooltipContent;

        protected override void Binding(BindingSet<SelectableItemView<GameStateItemViewModel>, GameStateItemViewModel> bindingSet)
        {
            base.Binding(bindingSet);
            bindingSet.Bind(m_Icon).For(v => v.sprite).To(vm => vm.Icon).OneWay();
        }
    }
}
