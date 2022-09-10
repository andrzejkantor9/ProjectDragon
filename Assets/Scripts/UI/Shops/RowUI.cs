using UnityEngine;
using UnityEngine.UI;

using TMPro;

using GameDevTV.Inventories;

using RPG.Shops;
using RPG.Debug;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        #region Config
        // [Header("CONFIG")]
        #endregion

        #region Cache
        [Header("CACHE")]
        // [Space(8f)]
        [SerializeField]
        private TextMeshProUGUI _nameField;
        [SerializeField]
        private Image _iconField;
        [SerializeField]
        private TextMeshProUGUI _availabilityField;
        [SerializeField]
        private TextMeshProUGUI _priceField;
        [SerializeField]
        private TextMeshProUGUI _quantityField;

        private Shop _currentShop;
        private ShopItem _shopItem;
        #endregion

        #region States
        #endregion

        #region Events
        // [Header("EVENTS")][Space(8f)]
        #endregion

        #region Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        #endregion

        #region PublicMethods
        public void Setup(Shop currentShop, ShopItem item)
        {
            _currentShop = currentShop;
            _shopItem = item;
            _quantityField.text = $"{item.QuantityInTransaction}";

            _iconField.sprite = item.Icon;
            _nameField.text = item.Name;
            _availabilityField.text = $"{item.Availability}";
            _priceField.text = $"${item.Price:N2}";
        }

        public void Add()
        {
            _currentShop.AddToTransaction(_shopItem.InventoryItem, 1);
        }

        public void Remove()
        {
            _currentShop.AddToTransaction(_shopItem.InventoryItem, -1);
        }
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
