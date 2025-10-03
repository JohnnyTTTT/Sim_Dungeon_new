using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using SoulGames.EasyGridBuilderPro;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Profiling.HierarchyFrameDataView;

namespace Johnny.SimDungeon
{
    [System.Serializable]
    public class DestroyToolViewModelProxy
    {
        public DestroyMode destroyMode;
        public Color background;
        public Sprite icon;
        public string description;
    }


    public class FixedToolbarViewModel : ListViewModel
    {
        public static string BeforeFixedToolbarSelectedItemChange = "BeforeFixedToolbarSelectedItemChange";
        public static string AfterFixedToolbarSelectedItemChange = "BeforeFixedToolbarSelectedItemChange";

        public bool Visibility
        {
            get
            {
                return m_Visibility;
            }
            set
            {
                Set(ref m_Visibility, value);
            }
        }
        private bool m_Visibility;

        private IDisposable m_Subscription;
        private IDisposable m_CategorySubscription;

        public FixedToolbarViewModel(IMessenger messenger) : base(messenger)
        {
            m_Subscription = Messenger.Subscribe<PropertyChangedMessage<GameState>>(OnGameStateChanged);
            m_CategorySubscription = Messenger.Subscribe<PropertyChangedMessage<CategoryObjectItemViewModel>>(OnCategoryObjectItemViewModelChanged);
        }

        protected override void OnSelectedItemChanged(SelectableItemViewModel oldItem, SelectableItemViewModel newItem)
        {
            Messenger.Publish(BeforeFixedToolbarSelectedItemChange, new PropertyChangedMessage<SelectableItemViewModel>(oldItem, newItem, nameof(newItem)));
            if (oldItem != null)
            {
                oldItem.OnSelectedChanged(false);
            }
            if (newItem != null)
            {
                newItem.OnSelectedChanged(true);
            }
            //Messenger.Publish(AfterFixedToolbarSelectedItemChange, new PropertyChangedMessage<SelectableItemViewModel>(oldItem, newItem, nameof(newItem)));
        }

        private void OnCategoryObjectItemViewModelChanged(PropertyChangedMessage<CategoryObjectItemViewModel> message)
        {
            if (message.NewValue != null)
            {
                SetSelectedItem(null);
            }
        }

        private void OnGameStateChanged(PropertyChangedMessage<GameState> message)
        {
            var value = message.NewValue;
            Visibility = value == GameState.Structure || value == GameState.Placement;
        }

        protected override void Dispose(bool disposing)
        {
            if (m_Subscription != null)
            {
                m_Subscription.Dispose();
                m_Subscription = null;
            }
            if (m_CategorySubscription != null)
            {
                m_CategorySubscription.Dispose();
                m_CategorySubscription = null;
            }
            Items?.Clear();
        }
    }

    public class FixedToolbarView : AnimationUIView
    {
        [SerializeField] private UIViewPositionAnimation m_AnimationPanel;
        [SerializeField] private ListView m_ListView;

        [SerializeField] private DestroyToolView m_DestroyEdgeToolItemView;
        [SerializeField] private DestroyToolView m_DestroyEntityToolItemView;

        private FixedToolbarViewModel m_ViewModel;

        protected override void Awake()
        {
            m_ViewModel = new FixedToolbarViewModel(Messenger.Default);
            this.SetDataContext(m_ViewModel);
        }

        protected override void Start()
        {
            m_ViewModel.AddItemAndInjectISelectCommand(m_DestroyEdgeToolItemView.GetDataContext() as SelectableItemViewModel);
            m_ViewModel.AddItemAndInjectISelectCommand(m_DestroyEntityToolItemView.GetDataContext() as SelectableItemViewModel);


            var bindingSet = this.CreateBindingSet<FixedToolbarView, FixedToolbarViewModel>();
            bindingSet.Bind(this).For(v => v.Visibility).To(vm => vm.Visibility).OneWay();
            //bindingSet.Bind(this.m_ListView).For(v => v.Items).To(vm => vm.Items).OneWay();
            bindingSet.Build();
        }

        public void AddViewModel(SelectableItemViewModel item)
        {

        }

        protected override void OnDestroy()
        {
            this.ClearAllBindings();
        }
    }
}
