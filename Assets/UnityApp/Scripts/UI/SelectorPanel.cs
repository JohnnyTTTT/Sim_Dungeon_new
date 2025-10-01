using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using SoulGames.EasyGridBuilderPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Johnny.SimDungeon
{
    public class SelectorPanel : MonoBehaviour
    {
        [SerializeField] private Button detroyButton;
        [SerializeField] private Button moveButton;
        private GridManager m_GridManager;
        private SelectionViewModel m_SelectionViewModel;
        private BuildableObjectSelector m_BuildableObjectSelector;
        private BuildableObjectMover m_BuildableObjectMover;
        private BuildableObjectDestroyer m_BuildableObjectDestroyer;

        protected void Start()
        {
            m_GridManager = GridManager.Instance;
            m_SelectionViewModel = BindingService.SelectionViewModel;

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
            StaticBinding();
        }



        private void StaticBinding()
        {
            var staticBindingSet = this.CreateBindingSet();
            staticBindingSet.Bind(this.gameObject).For(v => v.activeSelf).ToExpression(() => m_SelectionViewModel.SelectEntity != null);
            staticBindingSet.Build();
        }

        private void OnDetroyButtonClick()
        {
            m_BuildableObjectDestroyer.SetInputDestroyBuildableObject(m_SelectionViewModel.SelectEntity.buildableObject, true);
        }

        private void OnMoveButtonClick()
        {
            var buildableObject = m_SelectionViewModel.SelectEntity.buildableObject;
            var gridType = SpawnManager.Instance.GetGridTypeFromEasyGridBuilderPro(buildableObject.GetOccupiedGridSystem());
            BindingService.MainGameViewModel.GridType = gridType;
            //m_GridManager.SetActiveGridModeInAllGrids(GridMode.MoveMode);
            m_BuildableObjectMover.StartMovingObject(buildableObject, true);
        }
    }
}
