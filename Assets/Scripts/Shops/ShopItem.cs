using UnityEngine;

using GameDevTV.Inventories;

namespace RPG.Shops
{
    public class ShopItem
    {
        #region Config
        // [Header("CONFIG")]
        float _price;
        private InventoryItem _item;
        #endregion

        #region Cache
        // [Header("CACHE")][Space(8f)]
        #endregion

        #region States
        int _availability;
        int _quantityInTransaction;
        #endregion

        #region Events
        // [Header("EVENTS")][Space(8f)]
        #endregion

        #region Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Constructors
        public ShopItem(InventoryItem item, int availability, float price, int quantityInTransaction)
        {
            _item = item;
            _availability = availability;
            _price = price;
            _quantityInTransaction = quantityInTransaction;
        }
        #endregion

        #region EngineMethods
        #endregion

        #region PublicMethods
        public string Name => _item.GetDisplayName();
        public Sprite Icon => _item.GetIcon();
        public int Availability => _availability;
        
        public float Price => _price;
        public InventoryItem InventoryItem => _item;
        public int QuantityInTransaction => _quantityInTransaction;
        #endregion

        #region Interfaces
        #endregion

        #region StaticMethods
        #endregion

        #region Events
        #endregion

        #region PrivateMethods
        #endregion
    }
}