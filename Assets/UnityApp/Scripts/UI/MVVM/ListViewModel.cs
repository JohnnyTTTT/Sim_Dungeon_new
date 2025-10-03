using Loxodon.Framework.Commands;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using System;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class ListViewModel : ViewModelBase 
    {
        protected SimpleCommand<SelectableItemViewModel> ItemSelectCommand;

        public ObservableList<SelectableItemViewModel> Items
        {
            get { return this.m_Items; }
            set { this.Set(ref m_Items, value); }
        }
        private ObservableList<SelectableItemViewModel> m_Items;

        public ListViewModel(IMessenger messenger) : base( messenger)
        {
            Items = new ObservableList<SelectableItemViewModel>();
            ItemSelectCommand = new SimpleCommand<SelectableItemViewModel>(OnItemSelect);
        }

        protected SelectableItemViewModel SelectedItem
        {
            get
            {
                return m_SelectedItem;
            }
            private set
            {
                Set(ref m_SelectedItem, value);
            }
        }
        private SelectableItemViewModel m_SelectedItem;

        private void OnItemSelect(SelectableItemViewModel item)
        {
            if (SelectedItem == item)
            {
                SetSelectedItem(null);
            }
            else
            {
                SetSelectedItem(item);
            }
        }

        public void SetSelectedItem(SelectableItemViewModel item)
        {
            foreach (var i in m_Items)
            {
                i.IsSelected = false;
            }
            if (item != null)
            {
                item.IsSelected = true;
            }
            var oldSelectedItem = SelectedItem;
            SelectedItem = item;
            OnSelectedItemChanged(oldSelectedItem, SelectedItem);
            //if (notification)
            //{
            //    OnSelectedItemChanged?.Invoke(item);
            //}
        }

        public SelectableItemViewModel GetSelectedItem()
        {
            return SelectedItem;
        }

        protected virtual void OnSelectedItemChanged(SelectableItemViewModel old, SelectableItemViewModel item)
        {

        }

        public void ClearItem()
        {
            if (this.m_Items.Count <= 0)
                return;

            this.m_Items.Clear();
            SetSelectedItem(null);
        }

        public void AddItemAndInjectISelectCommand(SelectableItemViewModel item)
        {
            Items.Add(item);
            item.SetCommand(ItemSelectCommand);
        }

    }
}
