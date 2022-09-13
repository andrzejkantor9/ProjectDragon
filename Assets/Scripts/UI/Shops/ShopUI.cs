using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

using TMPro;

using Utilities;

using RPG.Core;
using RPG.Shops;
using RPG.Debug;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        Color _notPossibleToBuyColor = Color.red;
        [SerializeField]
        string _switchToSellText = "Switch to selling";
        [SerializeField]
        string _switchToBuyText = "Switch to buying";

        [SerializeField]
        string _buyButtonText = "Buy";
        [SerializeField]
        string _sellButtonText = "Sell";
        #endregion

        #region Cache
        [Header("CACHE")]
        [Space(8f)]
        [SerializeField]
        TextMeshProUGUI _shopName;
        [SerializeField]
        Transform _listRoot;
        [SerializeField]
        RowUI _rowPrefab;
        [SerializeField]
        TextMeshProUGUI _totalMoneyField;
        [SerializeField]
        Button _confirmButton;
        [SerializeField]
        Button _switchButton;

        Color _originalTotalMoneyColor;
        #endregion

        #region States
        Shopper _shopper;
        Shop _currentShop;
        #endregion

        #region Events
        // [Header("EVENTS")]
        //[Space(8f)]
        #endregion

        #region Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        void Awake()
        {
            AssertSerializedFields();

            _originalTotalMoneyColor = _totalMoneyField.color;
            _shopper = GameManager.PlayerGameObject.GetComponent<Shopper>();

            if(_shopper)
                _shopper.onActiveShopChange += ShopChanged;
            // Shop.onOpenShop += Activate;

            _confirmButton.onClick.AddListener(ConfirmTransaction);
            _switchButton.onClick.AddListener(SwitchMode);

            ShopChanged();
        }

        void OnEnable()
        {
            if(_currentShop)
                _currentShop.onChange += RefreshUI;
        }

        void OnDisable()
        {
            if(_currentShop)
                _currentShop.onChange -= RefreshUI;
        }
        
        void OnDestroy()
        {
            if(_shopper)
                _shopper.onActiveShopChange -= ShopChanged;
            // Shop.onOpenShop -= Activate;
            
            _confirmButton.onClick.RemoveListener(ConfirmTransaction);
            _switchButton.onClick.RemoveListener(SwitchMode);
        }

        // void Activate(Shop shop)
        // {
        //     _currentShop = shop;
        //     gameObject.SetActive(true);
        // }
        #endregion

        #region PublicMethods
        public void Close()
        {
            CustomLogger.Log($"close shop: {_shopName.text}", LogFrequency.Rare);
            _shopper.SetActiveShop(null);
        }

        public void ConfirmTransaction()
        {
            _currentShop.ConfirmTransaction();
        }

        public void SwitchMode()
        {
            _currentShop.SelectMode(!_currentShop.IsBuyingMode);
        }
        #endregion

        #region Interfaces
        #endregion

        #region StaticMethods
        #endregion

        #region Events
        void ShopChanged()
        {
            _currentShop = _shopper.ActiveShop;
            gameObject.SetActive(_currentShop != null);

            foreach(FilterButtonUI filterButton in GetComponentsInChildren<FilterButtonUI>())
            {
                filterButton.SetShop(_currentShop);
            }

            if(!_currentShop)
                return;
            _shopName.text = _currentShop.ShopName;

            RefreshUI();
        }
        #endregion

        #region PrivateMethods
        void RefreshUI()
        {
            _listRoot.GetComponent<DestroyObjects>().DestroyWithSpecifiedParameters();

            foreach(ShopItem item in _currentShop.GetFilteredItems())
            {
                var row = Instantiate<RowUI>(_rowPrefab, _listRoot);
                row.Setup(_currentShop, item);
            }

            _totalMoneyField.text = $"Total: ${_currentShop.GetTransactionTotal():N2}";
            _totalMoneyField.color = _currentShop.HasSufficientFunds() ? _originalTotalMoneyColor : _notPossibleToBuyColor;
            _confirmButton.interactable = _currentShop.CanTransact();

            TextMeshProUGUI switchText = _switchButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI buySellText = _confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            if(_currentShop.IsBuyingMode)
            {
                switchText.text = _switchToSellText;
                buySellText.text = _buyButtonText;
            }
            else
            {
                switchText.text = _switchToBuyText;
                buySellText.text = _sellButtonText;
            }

            foreach(FilterButtonUI filterButton in GetComponentsInChildren<FilterButtonUI>())
            {
                filterButton.RefreshUI();
            }
        }

        void AssertSerializedFields()
        {
            Assert.IsNotNull(_shopName, $"_shopName in {gameObject.name} is null");
            Assert.IsNotNull(_listRoot, $"_listRoot in {gameObject.name} is null");
            Assert.IsNotNull(_rowPrefab, $"_rowPrefab in {gameObject.name} is null");
            Assert.IsNotNull(_totalMoneyField, $"_totalField in {gameObject.name} is null");
            Assert.IsNotNull(_confirmButton, $"_confirmButton in {gameObject.name} is null");
        }
        #endregion
    }
}
