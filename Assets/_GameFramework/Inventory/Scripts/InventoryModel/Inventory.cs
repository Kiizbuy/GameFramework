using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using GameFramework.Inventory.Items;
using GameFramework.Extension;

namespace GameFramework.Inventory
{
    public sealed class Inventory : MonoBehaviour
    {
        public UnityEvent OnItemsStateChanged;
        public UnityEvent OnInventoryFull;

        [SerializeField, OneLine.OneLineWithHeader]
        private List<ItemState> _items = new List<ItemState>();

        public IReadOnlyCollection<ItemState> Items => _items;

        public ItemState GetItemStateViaIndex(int itemIndex)
        {
            if(itemIndex > _items.Count - 1 || itemIndex < 0)
            {
                Debug.LogError($"Index {itemIndex} is out of range", this);
                return null;
            }

            return _items[itemIndex];
        }

        public void AddItem(BaseItemData item, int count)
        {
            item.PutToInventory(this, count, (addableItem, countToPut) => PutNewItem(item, countToPut));
            OnItemsStateChanged?.Invoke();
        }

        private ItemState FindEmptyState()
            => _items.Where(state => state.ItemsCount == 0 || state.Data == null).FirstOrDefault();

        private ItemState PutNewItem(BaseItemData item, int count)
        {
            var state = FindEmptyState();

            if (state == null)
            {
                Debug.Log("Inventory is full");
                OnInventoryFull?.Invoke();
                return null;
            }

            state.Data = item;
            state.ItemsCount = count;

            return state;
        }

        private ItemState GetEjectableItem(BaseItemData item, int count)
        {
            return null;
        }

        public void ShuffleItems(int startIndex, int endIndex)
        {
            _items.Shuffle(startIndex, endIndex);
            OnItemsStateChanged?.Invoke();
        }

        public void SortItems()
        {
            _items = _items.OrderBy(x => -x.ItemsCount).ToList();
            OnItemsStateChanged?.Invoke();
        }
    }
}
