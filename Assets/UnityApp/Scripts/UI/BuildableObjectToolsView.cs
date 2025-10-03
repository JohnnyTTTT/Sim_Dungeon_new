using Loxodon.Framework.Binding;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.ViewModels;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Johnny.SimDungeon
{
    public class BuildableObjectToolsViewModel : ViewModelBase
    {
        public bool Visibility
        {
            get
            {
                return m_Visibility;
            }
            set
            {
                if (m_Visibility != value)
                {
                    Set(ref m_Visibility, value);
                }
            }
        }
        private bool m_Visibility;

        private IDisposable m_Subscription;

        public BuildableObjectToolsViewModel()
        {
            m_Subscription = Loxodon.Framework.Messaging.Messenger.Default.Subscribe<PropertyChangedMessage<BuildableGenItemViewModel>>(OnBuildableGenItemViewModelChanged);
        }

        private void OnBuildableGenItemViewModelChanged(PropertyChangedMessage<BuildableGenItemViewModel> message)
        {
            Visibility = message.NewValue != null;
        }

        public void OnRotateLeftButtonClicked()
        {
            SpawnManager.Instance.SetInputBuildableObjectClockwiseRotation();
        }

        public void OnRotateRightButtonClicked()
        {
            SpawnManager.Instance.SetInputBuildableObjectCounterClockwiseRotation();
        }

    }

    public class BuildableObjectToolsView : AnimationUIView
    {
        [SerializeField] private Button m_LeftButton;
        [SerializeField] private Button m_RightButton;


        private BuildableObjectToolsViewModel m_ViewModel;

        protected override void Awake()
        {
            m_ViewModel = new BuildableObjectToolsViewModel();
            this.SetDataContext(m_ViewModel);
        }

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<BuildableObjectToolsView, BuildableObjectToolsViewModel>();

            bindingSet.Bind(this).For(v => v.Visibility).To(vm => vm.Visibility).OneWay();
            bindingSet.Bind(this.m_LeftButton).For(v => v.onClick).To(vm => vm.OnRotateLeftButtonClicked);
            bindingSet.Bind(this.m_RightButton).For(v => v.onClick).To(vm => vm.OnRotateRightButtonClicked);

            bindingSet.Build();
        }
    }
}
