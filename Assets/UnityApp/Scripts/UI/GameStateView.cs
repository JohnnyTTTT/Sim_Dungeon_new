using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Views;
using System;
using UnityEngine;

namespace Johnny.SimDungeon
{
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

            public GameStateItemViewModel(GameState gameState, Sprite icon, ICommand selectCommand) : 
                base(selectCommand,null,icon)
            {
                m_GameState = gameState;
            }
        }

        //public class GameStateItemView : SelectableItemView<GameStateItemViewModel>
        //{
        //    [SerializeField] private Image m_Icon;
        //    [SerializeField] private TooltipContent m_TooltipContent;

        //    protected override void Binding(BindingSet<SelectableItemView<GameStateItemViewModel>, GameStateItemViewModel> bindingSet)
        //    {
        //        base.Binding(bindingSet);
        //        bindingSet.Bind(m_Icon).For(v => v.sprite).To(vm => vm.Icon).OneWay();
        //    }
        //}


        public class GameStateViewModel : ListViewModel
        {
            //private IDisposable m_Subscription;

            public GameStateViewModel(IMessenger messenger) : base(messenger)
            {
                //m_Subscription = Loxodon.Framework.Messaging.Messenger.Default.Subscribe<PropertyChangedMessage<GameState>>(OnGameStateChanged);
            }


            protected override void OnSelectedItemChanged(SelectableItemViewModel old, SelectableItemViewModel item)
            {
                if (item != null)
                {
                    GameStateManager.Instance.ChangeState((item as GameStateItemViewModel).GameState);
                }
                //else
                //{
                //    GameStateManager.Instance.ChangeState(GameState.Default);
                //}
            }


            //private void OnGameStateChanged(PropertyChangedMessage<GameState> message)
            //{
            //    var state = message.NewValue;
            //    foreach (var item in Items)
            //    {
            //        item.IsSelected = item.GameState == state;
            //    }
            //}

            public GameStateItemViewModel CreateViewMode(GameState gameState, Sprite icon)
            {
                var item = new GameStateItemViewModel(gameState, icon, this.ItemSelectCommand);
                Items.Add(item);
                return item;
            }
        }

        public class GameStateView : UIView
        {
            //[SerializeField] private AnimationPanel m_AnimationPanel;
            [SerializeField] private ListView m_ListView;
            [SerializeField] private Sprite m_DefalutIcon;
            [SerializeField] private Sprite m_StructureIcon;
            [SerializeField] private Sprite m_PlacementIcon;

            private GameStateViewModel m_ViewModel;

            protected override void Awake()
            {
                m_ViewModel = new GameStateViewModel(Messenger.Default);
                this.SetDataContext(m_ViewModel);
                var serviceContainer = Context.GetApplicationContext().GetContainer();
                serviceContainer.Register(m_ViewModel);
            }

            protected override void Start()
            {
                m_ViewModel.CreateViewMode(GameState.Default, m_DefalutIcon);
                m_ViewModel.CreateViewMode(GameState.Structure, m_StructureIcon);
                m_ViewModel.CreateViewMode(GameState.Placement, m_PlacementIcon);

                var bindingSet = this.CreateBindingSet<GameStateView, GameStateViewModel>();

                //bindingSet.Bind(this.m_AnimationPanel).For(v => v.Show).To(vm => vm.IsVisible).OneWay();
                bindingSet.Bind(this.m_ListView).For(v => v.Items).To(vm => vm.Items).OneWay();

                bindingSet.Build();
            }

            protected override void OnDestroy()
            {
                this.ClearAllBindings();
            }
        }
    }
}
