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
    public class FixedToolbarViewModel : ListViewModel<DestroyToolViewModel>
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

        public FixedToolbarViewModel(IMessenger messenger) : base(messenger)
        {
            m_Subscription = Messenger.Subscribe<PropertyChangedMessage<GameState>>(OnGameStateChanged);
            //m_Subscription = Messenger.Subscribe<PropertyChangedMessage<CategoryObjectItemViewModel>>(OnCategoryObjectItemViewModelChanged);
        }

        //protected override void OnSelectedItemChanged(SelectableItemViewModel old, SelectableItemViewModel item)
        //{
        //    Debug.Log(Items.Count);
        //    //Messenger.Publish(nameof(FixedToolbarViewModel), new PropertyChangedMessage<SelectableItemViewModel>(old, item, nameof(SelectableItemViewModel)));
        //}

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
            IsVisible = value == GameState.Structure || value == GameState.Placement;
        }
    }

    public class FixedToolbarView : AnimationUIView
    {
        [SerializeField] private UIViewPositionAnimation m_AnimationPanel;
        [SerializeField] private ListView m_ListView;

        //[SerializeField] private DestroyToolItemView m_DestroyEdgeToolItemView;
        //[SerializeField] private DestroyToolItemView m_DestroyEntityToolItemView;

        private FixedToolbarViewModel m_ViewModel;

        protected override void Awake()
        {
            m_ViewModel = new FixedToolbarViewModel(Messenger.Default);

            this.SetDataContext(m_ViewModel);


        }

        protected override void Start()
        {
            StartCoroutine(LoadWorld());
            var bindingSet = this.CreateBindingSet<FixedToolbarView, FixedToolbarViewModel>();
            //bindingSet.Bind(this.m_ListView).For(v => v.Items).To(vm => vm.Items).OneWay();
            //bindingSet.Bind(this.m_ListView).For(v => v.Items).To(vm => vm.Items).OneWay();
            bindingSet.Build();
        }
        private IEnumerator LoadWorld()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            ////var a = m_DestroyEntityToolItemView.CreateDataContext();
            ////var b = m_DestroyEdgeToolItemView.CreateDataContext();
            //Debug.Log(a.GetHashCode());
            //yield return new WaitForEndOfFrame();
            ////m_ViewModel.AddItem(a);
            ////m_ViewModel.AddItem(b);
            //Debug.Log(m_ViewModel.Items.Count);
            //yield return new WaitForEndOfFrame();
            //Debug.Log(m_DestroyEntityToolItemView.GetDataContext().GetHashCode());
            ////((FixedToolbarViewModel)this.GetDataContext()).Items.Add(new DestroyToolViewModel(DestroyMode.Edge));
        }
    }
}
