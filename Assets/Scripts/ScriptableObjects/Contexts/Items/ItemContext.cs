using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RPG.ScriptableObjects.Contexts
{
    public abstract class BaseItemContext : ScriptableObjectConfiguration 
    {
        [SerializeField]
        private ItemType _itemType;
        [SerializeField]
        private RarityItemType _itemRarity;

        public ItemType Type
        {
            get => _itemType;
#if UNITY_EDITOR
            set => _itemType = value;
#endif
        }
        public RarityItemType Rarity
        {
            get => _itemRarity;
#if UNITY_EDITOR
            set => _itemRarity = value;
#endif
        }

        public abstract List<BaseItem> GetItems();
        public abstract BaseItem TryGetItemByID(string id);
    }

    public abstract class BaseItemContext<T> : BaseItemContext where T : BaseItem
    {
        [SerializeField]
        private List<T> _items = new List<T>();

#if UNITY_EDITOR
        /// <summary>
        /// Only Editor
        /// </summary>
        public List<T> Items { get => _items; set => _items = value; }
#endif

        public override List<BaseItem> GetItems()
        {
            var list = new List<BaseItem>(_items.Count);

            foreach (var element in _items) list.Add(element.Clone());
            return list;
        }

        public override BaseItem TryGetItemByID(string id)
        {
            foreach(var item in _items)
            {
                if (item.ID == id) return item.Clone();
			}
            return null;
		}
    }
}
