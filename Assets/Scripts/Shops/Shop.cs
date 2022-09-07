using System;
using System.Collections.Generic;

using UnityEngine;

using GameDevTV.Inventories;

using RPG.Interactions;
using RPG.Core;
using RPG.Debug;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable
    {
        #region Config
        // [Header("CONFIG")]
        #endregion

        #region Cache
        // [Header("CACHE")]
        // [Space(8f)]
        #endregion

        #region States
        #endregion

        #region Events
        // [Header("EVENTS")][Space(8f)]
        public event Action onChange;
        #endregion

        #region Statics
        #endregion

        #region Data
        public class ShopItem
        {
            InventoryItem _item;
            int _availability;
            float _price;
            int _quantityInTransaction;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        #endregion

        #region PublicMethods
        public IEnumerable<ShopItem> FilteredItems => null;
        public ItemCategory Filter => ItemCategory.None;
        public bool isBuyingMode => true;
        public bool CanTransact => true;
        public float TransactionTotal => 0f;
        
        public void SelectFilter(ItemCategory category)
        {
        }

        public void SelectMode(bool isBuying)
        {
        }

        public void ConfirmTransaction()
        {
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
        }
        #endregion

        #region Interfaces
        public CursorType GetCursorType() => CursorType.Shop;

        public bool HandleRaycast(GameObject playerController)
        {
            if(InputManager.WasPointerPressedThisFrame())
            {
                CustomLogger.Log("pointer clicked on shopper", LogFrequency.Rare);
                playerController.GetComponent<Shopper>().SetActiveShop(this);
                if(onChange != null)
                    onChange();
            }
            return true;
        }
        #endregion

        #region StaticMethods
        #endregion

        #region Events
        #endregion

        #region PrivateMethods
        #endregion
    }
}
