using Loxodon.Framework.Commands;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using System;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class ListViewModel<T> : ViewModelBase where T : SelectableItemViewModel
    {
        protected SimpleCommand<T> ItemSelectCommand;
        protected SimpleCommand<T> ItemClickCommand;
        public ObservableList<T> Items
        {
            get { return this.m_Items; }
            set { this.Set(ref m_Items, value); }
        }
        private ObservableList<T> m_Items;

        public ListViewModel()
        {
            Items = new ObservableList<T>();
            ItemSelectCommand = new SimpleCommand<T>(OnItemSelect);
            ItemClickCommand = new SimpleCommand<T>(OnItemClick);
        }

        public T SelectedItem
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
        private T m_SelectedItem;

        protected virtual void OnItemClick(T item)
        {

        }

        private void OnItemSelect(T item)
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

        public void SetSelectedItem(T item)
        {
            foreach (var i in Items)
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

        protected virtual void OnSelectedItemChanged(T old, T item)
        {

        }
    }
}
