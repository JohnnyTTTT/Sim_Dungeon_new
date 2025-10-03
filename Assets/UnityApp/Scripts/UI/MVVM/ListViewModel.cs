using Loxodon.Framework.Commands;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using System;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class ListViewModel<T> : ViewModelBase where T : SelectableItemViewModel
    {
        protected SimpleCommand<T> ItemSelectCommand;

        public ObservableList<T> Items
        {
            get { return this.m_Items; }
            set { this.Set(ref m_Items, value); }
        }
        private ObservableList<T> m_Items;

        public ListViewModel(IMessenger messenger) : base( messenger)
        {
            Items = new ObservableList<T>();
            ItemSelectCommand = new SimpleCommand<T>(OnItemSelect);
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

        protected virtual void OnSelectedItemChanged(T old, T item)
        {

        }

        public void ClearItem()
        {
            if (this.m_Items.Count <= 0)
                return;

            this.m_Items.Clear();
            SetSelectedItem(null);
        }

        public void AddItem(T item)
        {
            Debug.Log(item);
            item.SetSelectCommand(ItemSelectCommand) ;
            Items.Add(item);
        }
    }
}
