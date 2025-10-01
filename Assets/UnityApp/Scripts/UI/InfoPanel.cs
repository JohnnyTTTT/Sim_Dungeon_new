using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.ViewModels;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Johnny.SimDungeon
{
    public class SelectionViewModel : ViewModelBase
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
                Set(ref m_SmallCell, value);
            }
        }
        private Element_SmallCell m_SmallCell;

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

        public string Title
        {
            get
            {
                return HoverSmallCell != null ? HoverSmallCell.ToString() : "No SamllCell";
            }
        }

        public string CoordText
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

    }


    public class InfoPanel : ViewBase<SelectionViewModel>
    {
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_CoordText;
        [SerializeField] private TextMeshProUGUI m_RegionText;


        protected override void Start()
        {
            ViewModel = BindingService.SelectionViewModel;
            base.Start();
        }

        protected override void Binding(BindingSet<ViewBase<SelectionViewModel>, SelectionViewModel> bindingSet)
        {
            bindingSet.Bind(this.gameObject).For(v => v.activeSelf).ToExpression(vm => vm.HoverSmallCell != null).OneWay();
            bindingSet.Bind(this.m_Title).For(v => v.text).To(vm => vm.Title).OneWay();
            bindingSet.Bind(this.m_CoordText).For(v => v.text).To(vm => vm.CoordText).OneWay();
            bindingSet.Bind(this.m_RegionText).For(v => v.text).To(vm => vm.Region).OneWay();
        }
    }
}
