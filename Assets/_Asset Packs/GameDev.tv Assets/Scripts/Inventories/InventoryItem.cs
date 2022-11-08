using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// A ScriptableObject that represents any item that can be put in an
    /// inventory.
    /// </summary>
    /// <remarks>
    /// In practice, you are likely to use a subclass such as `ActionItem` or
    /// `EquipableItem`.
    /// </remarks>
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Config
        [SerializeField]
        [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        private string itemID = null;
        [SerializeField] [Tooltip("Item name to be displayed in UI.")]
        private string displayName = null;
        [SerializeField] [TextArea] [Tooltip("Item description to be displayed in UI.")]
        private string description = null;
        [SerializeField] [Tooltip("The UI icon to represent this item in the inventory.")]
        private Sprite icon = null;
        [SerializeField] [Tooltip("The prefab that should be spawned when this item is dropped.")]
        private Pickup pickup = null;
        [SerializeField] [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")]
        private bool stackable = false;

        [SerializeField]
        private float _price;
        [SerializeField]
        private ItemCategory _category = ItemCategory.None;
        #endregion

        #region States
        static Dictionary<string, InventoryItem> itemLookupCache;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Public
        public float Price => _price;
        public ItemCategory Category => _category;

        /// <summary>
        /// Spawn the pickup gameobject into the world.
        /// </summary>
        /// <param name="position">Where to spawn the pickup.</param>
        /// <param name="number">How many instances of the item does the pickup represent.</param>
        /// <returns>Reference to the pickup object spawned.</returns>
        public Pickup SpawnPickup(Vector3 position, int number)
        {
            var pickup = Instantiate(this.pickup);
            pickup.transform.position = position;
            pickup.Setup(this, number);
            return pickup;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public string GetItemID()
        {
            return itemID;
        }

        public bool IsStackable()
        {
            return stackable;
        }
        
        public string GetDisplayName()
        {
            return displayName;
        }

        public string GetDescription()
        {
            return description;
        }
        #endregion

        #region Events & Statics
        /// <summary>
        /// Get the inventory item instance from its UUID.
        /// </summary>
        /// <param name="itemID">
        /// String UUID that persists between game instances.
        /// </param>
        /// <returns>
        /// Inventory item instance corresponding to the ID.
        /// </returns>
        public static InventoryItem GetFromID(string itemID)
        {
            if (itemLookupCache == null)
            {
                itemLookupCache = new Dictionary<string, InventoryItem>();
                var itemList = Resources.LoadAll<InventoryItem>("");
                foreach (var item in itemList)
                {
                    if (itemLookupCache.ContainsKey(item.itemID))
                    {
                        Debug.LogError(string.Format("Looks like there's a duplicate GameDevTV.UI.InventorySystem ID for objects: {0} and {1}", itemLookupCache[item.itemID], item));
                        continue;
                    }

                    itemLookupCache[item.itemID] = item;
                }
            }

            if (itemID == null || !itemLookupCache.ContainsKey(itemID)) return null;
            return itemLookupCache[itemID];
        }
        #endregion

        #region Private
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(itemID))
            {
                itemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Required by the ISerializationCallbackReceiver but we don't need to do anything with it.
        }
        #endregion
    }
}
