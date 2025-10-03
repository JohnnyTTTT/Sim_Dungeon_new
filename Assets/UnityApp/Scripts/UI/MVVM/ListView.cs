using Loxodon.Framework.Binding;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using System.Collections.Specialized;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class ListView : UIView
    {
        public Transform content;
        public GameObject itemTemplate;

        public ObservableList<SelectableItemViewModel> Items
        {
            get { return this.m_Items; }
            set
            {
                if (this.m_Items == value)
                    return;

                if (this.m_Items != null)
                    this.m_Items.CollectionChanged -= OnCollectionChanged;

                this.m_Items = value;
                this.OnItemsChanged();

                if (this.m_Items != null)
                    this.m_Items.CollectionChanged += OnCollectionChanged;
            }
        }
        private ObservableList<SelectableItemViewModel> m_Items;

        private SelectableItemViewModel m_CurrentSelect;

        protected override void OnDestroy()
        {
            if (this.m_Items != null)
                this.m_Items.CollectionChanged -= OnCollectionChanged;
        }

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddItem(eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.RemoveItem(eventArgs.OldStartingIndex, eventArgs.OldItems[0]);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.ReplaceItem(eventArgs.OldStartingIndex, eventArgs.OldItems[0], eventArgs.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ResetItem();
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.MoveItem(eventArgs.OldStartingIndex, eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                    break;
            }
        }

        protected virtual void OnItemsChanged()
        {
            int count = this.content.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                var child = this.content.GetChild(i);
                GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < this.m_Items.Count; i++)
            {
                this.AddItem(i, m_Items[i]);
            }
        }

        protected virtual void AddItem(int index, object item)
        {
            var itemViewGo = Instantiate(this.itemTemplate);
            itemViewGo.transform.SetParent(this.content, false);
            itemViewGo.transform.SetSiblingIndex(index);
            itemViewGo.SetActive(true);

            var itemView = itemViewGo.GetComponent<UIView>();
            itemView.SetDataContext((SelectableItemViewModel)item);
        }

        protected virtual void RemoveItem(int index, object item)
        {
            var transform = this.content.GetChild(index);
            var itemView = transform.GetComponent<UIView>();
            if (itemView.GetDataContext() == item)
            {
                itemView.gameObject.SetActive(false);
                Destroy(itemView.gameObject);
            }
        }

        protected virtual void ReplaceItem(int index, object oldItem, object item)
        {
            var transform = this.content.GetChild(index);
            var itemView = transform.GetComponent<UIView>();
            if (itemView.GetDataContext() == oldItem)
            {
                itemView.SetDataContext(item);
            }
        }

        protected virtual void MoveItem(int oldIndex, int index, object item)
        {
            var transform = this.content.GetChild(oldIndex);
            var itemView = transform.GetComponent<UIView>();
            itemView.transform.SetSiblingIndex(index);
        }

        protected virtual void ResetItem()
        {
            for (int i = this.content.childCount - 1; i >= 0; i--)
            {
                var transform = this.content.GetChild(i);
                Destroy(transform.gameObject);
            }
        }

    }
}
