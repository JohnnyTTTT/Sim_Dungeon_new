using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using SoulGames.EasyGridBuilderPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Profiling.HierarchyFrameDataView;

namespace Johnny.SimDungeon
{
    public class FixedToolbarViewModel : ListViewModel<SelectableItemViewModel>
    {
        private IDisposable m_Subscription;

        public bool IsVisible
        {
            get
            {
                return m_IsVisible;
            }
            set
            {
                Set(ref m_IsVisible, value);
            }
        }
        private bool m_IsVisible;

        public FixedToolbarViewModel()
        {
            m_Subscription = Loxodon.Framework.Messaging.Messenger.Default.Subscribe<PropertyChangedMessage<GameState>>(OnGameStateChanged);
        }

        private void OnGameStateChanged(PropertyChangedMessage<GameState> message)
        {
            var value = message.NewValue;
            IsVisible = value == GameState.Structure || value == GameState.Placement;
        }
    }

    public class FixedToolbarView : AnimationUIView
    {
        [SerializeField] private UIViewPositionAnimation m_AnimationPanel;

        private FixedToolbarViewModel m_ViewModel;

        protected override void Awake()
        {
            m_ViewModel = new FixedToolbarViewModel();
            this.SetDataContext(m_ViewModel);
        }

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<FixedToolbarView, FixedToolbarViewModel>();
            //bindingSet.Bind(this.m_AnimationPanel).For(v => v.Show).To(vm => vm.IsVisible).OneWay();
            bindingSet.Build();
        }
    }
}
