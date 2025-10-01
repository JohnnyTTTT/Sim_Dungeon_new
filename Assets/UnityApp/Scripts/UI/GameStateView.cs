using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Views;
using System;
using UnityEngine;

namespace Johnny.SimDungeon
{

    public class GameStateViewModel : ListViewModel<GameStateItemViewModel>
    {
        //private IDisposable m_Subscription;

        public GameStateViewModel()
        {
            //m_Subscription = Loxodon.Framework.Messaging.Messenger.Default.Subscribe<PropertyChangedMessage<GameState>>(OnGameStateChanged);
        }


        protected override void OnSelectedItemChanged(GameStateItemViewModel old, GameStateItemViewModel item)
        {
            if (item != null)
            {
                GameStateManager.Instance.ChangeState(item.GameState);
            }
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
        [SerializeField] private GameStateListView m_ListView;
        [SerializeField] private Sprite m_DefalutIcon;
        [SerializeField] private Sprite m_StructureIcon;
        [SerializeField] private Sprite m_PlacementIcon;

        private GameStateViewModel m_ViewModel;

        protected override void Awake()
        {
            m_ViewModel = new GameStateViewModel();
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
    }
}
