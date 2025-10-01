using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using SoulGames.EasyGridBuilderPro;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Johnny.SimDungeon
{
    public class SelectionViewModel : ViewModelBase
    {
        public Entity HoverEntity
        {
            get
            {
                return m_HoverEntity;
            }
            set
            {
                Set(ref m_HoverEntity, value);
                RaisePropertyChanged();
            }
        }
        private Entity m_HoverEntity;

        public Entity SelectEntity
        {
            get
            {
                return m_SelectEntity;
            }
            set
            {
                Set(ref m_SelectEntity, value);
            }
        }
        private Entity m_SelectEntity;
    }

    public class SelectorView : UIView
    {
        [SerializeField] private Button detroyButton;
        [SerializeField] private Button moveButton;

        private GridManager m_GridManager;
        private SelectionViewModel m_SelectionViewModel;
        private MainGameViewModel m_MainGameViewModel;
        private BuildableObjectSelector m_BuildableObjectSelector;
        private BuildableObjectMover m_BuildableObjectMover;
        private BuildableObjectDestroyer m_BuildableObjectDestroyer;

        protected override void Awake()
        {
            m_SelectionViewModel = new SelectionViewModel();
            this.SetDataContext(m_SelectionViewModel);

            var serviceContainer = Context.GetApplicationContext().GetContainer();
            serviceContainer.Register(m_SelectionViewModel);
        }

        protected override void Start()
        {
            m_GridManager = GridManager.Instance;
            if (m_GridManager.TryGetBuildableObjectSelector(out var buildableObjectSelector))
            {
                m_BuildableObjectSelector = buildableObjectSelector;
            }
            if (m_GridManager.TryGetBuildableObjectDestroyer(out var buildableObjectDestroyer))
            {
                m_BuildableObjectDestroyer = buildableObjectDestroyer;
            }
            if (m_GridManager.TryGetBuildableObjectMover(out BuildableObjectMover buildableObjectMover))
            {
                m_BuildableObjectMover = buildableObjectMover;

            }

            detroyButton.onClick.AddListener(OnDetroyButtonClick);
            moveButton.onClick.AddListener(OnMoveButtonClick);


            var serviceContainer = Context.GetApplicationContext().GetContainer();
            m_MainGameViewModel = serviceContainer.Resolve<MainGameViewModel>();


            var bindingSet = this.CreateBindingSet<SelectorView, SelectionViewModel>();
            bindingSet.Bind(this.gameObject).For(v => v.activeSelf).ToExpression((vm) => vm.SelectEntity != null);
            bindingSet.Build();
        }

        private void OnDetroyButtonClick()
        {
            m_BuildableObjectDestroyer.SetInputDestroyBuildableObject(m_SelectionViewModel.SelectEntity.buildableObject, true);
        }

        private void OnMoveButtonClick()
        {
            var buildableObject = m_SelectionViewModel.SelectEntity.buildableObject;
            var gridType = SpawnManager.Instance.GetGridTypeFromEasyGridBuilderPro(buildableObject.GetOccupiedGridSystem());
            SpawnManager.Instance.SetGridType(gridType);
            //m_GridManager.SetActiveGridModeInAllGrids(GridMode.MoveMode);
            m_BuildableObjectMover.StartMovingObject(buildableObject, true);
        }
    }
}
