using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.ViewModels;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using SoulGames.EasyGridBuilderPro;
using System;

namespace Johnny.SimDungeon
{
    public class CellInfoViewModel : ViewModelBase
    {
        public Element_LargeCell HoverLargeCell
        {
            get
            {
                return m_HoverLargeCell;
            }
            set
            {
                Set(ref m_HoverLargeCell, value);
            }
        }
        private Element_LargeCell m_HoverLargeCell;

        public Element_SmallCell HoverSmallCell
        {
            get
            {
                return m_SmallCell;
            }
            set
            {
                if (Set(ref m_SmallCell, value))
                {
                    RaisePropertyChanged(nameof(SmallCellTitle));
                    RaisePropertyChanged(nameof(SmallCellCoordText));
                }
                //Set(ref m_SmallCell, value);
                //RaisePropertyChanged();
            }
        }
        private Element_SmallCell m_SmallCell;

        public Region Region
        {
            get
            {
                return m_Region;
            }
            set
            {
                Set(ref m_Region, value);
                RaisePropertyChanged();
            }
        }
        private Region m_Region;

        public string SmallCellTitle
        {
            get
            {
                return HoverSmallCell != null ? HoverSmallCell.ToString() : "No SamllCell";
            }
        }

        public string SmallCellCoordText
        {
            get
            {
                return HoverSmallCell != null ? HoverSmallCell.coord.ToString() : Vector2Int.zero.ToString();
            }
        }

        public string RegionName
        {
            get
            {
                return Region != null  ? Region.name : "No Region";
            }
        }

        public CellInfoViewModel()
        {
            SelectionManager.Instance.OnSmallCellHover += OnSmallCellHover;
            SelectionManager.Instance.OnLargeCellHover += OnLargeCellHover;
        }

        private void OnLargeCellHover(Element_LargeCell cell)
        {
            HoverLargeCell = cell;
        }

        private void OnSmallCellHover(Element_SmallCell cell)
        {
            HoverSmallCell = cell;
        }
    }

    public class InfoPanel : UIView
    {
        private CellInfoViewModel m_CellInfoViewModel;
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_CoordText;
        [SerializeField] private TextMeshProUGUI m_RegionText;

        protected override void Awake()
        {
            m_CellInfoViewModel = new CellInfoViewModel();
            this.SetDataContext(m_CellInfoViewModel);
            var serviceContainer = Context.GetApplicationContext().GetContainer();
            serviceContainer.Register(m_CellInfoViewModel);
        }

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<InfoPanel, CellInfoViewModel>();
            //bindingSet.Bind(this.gameObject).For(v => v.activeSelf).ToExpression((vm) => vm.SelectEntity != null);

            bindingSet.Bind(this.gameObject).For(v => v.activeSelf).ToExpression(vm => vm.HoverSmallCell != null).OneWay();
            bindingSet.Bind(this.m_Title).For(v => v.text).To(vm => vm.SmallCellTitle).OneWay();
            bindingSet.Bind(this.m_CoordText).For(v => v.text).To(vm => vm.SmallCellCoordText).OneWay();
            bindingSet.Bind(this.m_RegionText).For(v => v.text).To(vm => vm.Region).OneWay();

            bindingSet.Build();
        }
    }
}
