using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

using GameDevTV.Inventories;

using RPG.Interactions;
using RPG.Core;
using RPG.Inventories;
using RPG.Debug;
using RPG.Stats;
using RPG.Saving;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        string _shopName;
        [SerializeField]
        StockItemConfig[] _stockConfig;
        [Tooltip("more = lower price")]
        [SerializeField][Range(-1000f, 1000f)]
        float _sellingPercentage = 50f;
        [SerializeField][Range(0f, 100f)]
        float _maxBarterDiscountPercentage = 80f;
        #endregion

        #region Cache
        // [Header("CACHE")]
        // [Space(8f)]
        Shopper _currentShopper;
        #endregion

        #region States
        Dictionary<InventoryItem, int> _transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> _stockSold = new Dictionary<InventoryItem, int>();

        bool _isBuyingMode = true;
        ItemCategory _filter = ItemCategory.None;
        #endregion

        #region Events
        // [Header("EVENTS")][Space(8f)]
        public event Action onChange;
        public static event Action<Shop> onOpenShop;
        #endregion

        #region Statics
        #endregion

        #region Data
        //make variables if class becomes public
        [System.Serializable]
        class StockItemConfig
        {
            public InventoryItem Item;
            public int InitialStock = 0;
            [Range(-1000f, 1000f)]
            public float BuyingDiscountPercentage = 0f;
            public int LevelToUnlock = 0;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        void Awake()
        {
            AssertSerializedFields();
        }
        #endregion

        #region PublicMethods
        public ItemCategory Filter => _filter;
        public bool IsBuyingMode => _isBuyingMode;
        public string ShopName => _shopName;

        public void SetShopper(Shopper shopper) => _currentShopper = shopper;

        public bool CanTransact()
        {
            if(IsTransactionEmpty())
                return false;
            if(!HasSufficientFunds())
                return false;
            if(!HasInventorySpace())
                return false;

            return true;
        }

        public float GetTransactionTotal()
        {
            float total = 0;
            foreach(ShopItem item in GetAllItems())
            {
                total += item.Price * item.QuantityInTransaction;
            }

            return total;
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem inventoryItem = shopItem.InventoryItem;
                if(_filter == ItemCategory.None || inventoryItem.Category == _filter)
                    yield return shopItem;
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            Dictionary<InventoryItem, float> prices = GetPrices();
            Dictionary<InventoryItem, int> availabilities = GetAvailabilities();

            foreach(InventoryItem inventoryItem in availabilities.Keys)
            {
                if(availabilities[inventoryItem] <= 0)
                    continue;

                float price = prices[inventoryItem];
                int quantityInTransaction = 0;

                _transaction.TryGetValue(inventoryItem, out quantityInTransaction);
                int availability = availabilities[inventoryItem];

                yield return new ShopItem(inventoryItem, availability, price, quantityInTransaction);
            }
        }

        public void SelectFilter(ItemCategory category)
        {
            _filter = category;
            CustomLogger.Log($"current category: {category.ToString()}", LogFrequency.Rare);

            if(onChange != null)
                onChange();
        }

        public void SelectMode(bool isBuying)
        {
            _isBuyingMode = isBuying;
            if(onChange != null)
                onChange();
        }

        public void ConfirmTransaction()
        {
            Inventory shopperInventory = _currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = _currentShopper.GetComponent<Purse>();
            if(!shopperInventory || !shopperPurse)
                return;

            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.InventoryItem;
                int quantity = shopItem.QuantityInTransaction;
                float price = shopItem.Price;

                for (int i = 0; i < quantity; i++)
                {
                    if(_isBuyingMode)
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }
                }
            }

            if(onChange != null)
                onChange();
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            CustomLogger.Log($"Added to transaction: {item.GetDisplayName()} x {quantity} ", LogFrequency.Regular);

            if(!_transaction.ContainsKey(item))
            {
                _transaction[item] = 0;
            }

            Dictionary<InventoryItem, int> availabilities = GetAvailabilities();
            int availability = availabilities[item];
            if(_transaction[item] + quantity > availability)
            {
                _transaction[item] = availability;
            }
            else
            {
                _transaction[item] += quantity;
            }

            if(_transaction[item] <= 0)
            {
                _transaction.Remove(item);
            }

            if(onChange != null)
                onChange();
        }

        public bool HasSufficientFunds()
        {
            if(!_isBuyingMode)
                return true;

            Purse purse = _currentShopper.GetComponent<Purse>();
            if(!purse)
                return false;

            return purse.Balance >= GetTransactionTotal();
        }

        public bool HasInventorySpace()
        {
            if(!_isBuyingMode)
                return true;

            Inventory shopperInventory = _currentShopper.GetComponent<Inventory>();
            if(!shopperInventory)
                return false;
            var flatItems = new List<InventoryItem>();

            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.InventoryItem;
                int quantity = shopItem.QuantityInTransaction;

                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            bool hasSpaceForItems = shopperInventory.HasSpaceFor(flatItems);
            return hasSpaceForItems;
        }
        #endregion

        #region Interfaces
        public CursorType GetCursorType() => CursorType.Shop;

        public bool HandleRaycast(GameObject playerController)
        {
            if(InputManager.WasPointerPressedThisFrame())
            {
                CustomLogger.Log($"pointer clicked on shopper: {gameObject.name}", LogFrequency.Rare);
                playerController.GetComponent<Shopper>().SetActiveShop(this);

                if(onOpenShop != null)
                    onOpenShop(this);
                if(onChange != null)
                    onChange();
            }
            return true;
        }

        public object CaptureState()
        {
            Dictionary<string, int> soldObjects = new Dictionary<string, int>();

            foreach(KeyValuePair<InventoryItem, int> pair in _stockSold)
            {
                soldObjects[pair.Key.GetItemID()] = pair.Value;
            }

            return soldObjects;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> soldObjects = (Dictionary<string, int>) state;
            _stockSold.Clear();

            foreach(KeyValuePair<string, int> soldObject in soldObjects)
            {
                InventoryItem inventoryItem = InventoryItem.GetFromID(soldObject.Key);
                _stockSold[inventoryItem] = soldObject.Value;
            }
        }
        #endregion

        #region StaticMethods
        #endregion

        #region Events
        #endregion

        #region PrivateMethods
        void AssertSerializedFields()
        {
            Assert.IsTrue(!String.IsNullOrWhiteSpace(_shopName), $"_shopName in {gameObject.name} is empty");
        }

        bool IsTransactionEmpty()
        {
            return _transaction.Count == 0;
        }

        int CountItemsInInventory(InventoryItem item)
        {
            Inventory inventory = _currentShopper.GetComponent<Inventory>();
            if(inventory == null)
                return 0;

            int total = 0;
            for(int i=0; i < inventory.GetSize(); ++i)
            {
                if(inventory.GetItemInSlot(i) == item)
                {
                    total += inventory.GetNumberInSlot(i);
                }
            }

            return total;
        }

        void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if(shopperPurse.Balance < price)
                return;

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if(success)
            {
                AddToTransaction(item, -1);

                if(!_stockSold.ContainsKey(item))
                {
                    _stockSold[item] = 0;
                }
                ++_stockSold[item];
                shopperPurse.UpdateBalance(-price);
            }
        }

        void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if(slot == -1)
                return;

            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);

            if(!_stockSold.ContainsKey(item))
            {
                _stockSold[item] = 0;
            }
            --_stockSold[item];
            shopperPurse.UpdateBalance(price);
        }

        int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if(shopperInventory.GetItemInSlot(i) == item)
                    return i;
            }

            return -1;
        }

        int GetShopperLevel()
        {
            var stats = _currentShopper.GetComponent<BaseStats>();
            if(!stats)
                return 0;

            return stats.GetLevel();
        }

        Dictionary<InventoryItem, int> GetAvailabilities()
        {
            var availabilities = new Dictionary<InventoryItem, int>();

            foreach(StockItemConfig config in GetAvailableConfigs())
            {
                if(_isBuyingMode)
                {
                    if(!availabilities.ContainsKey(config.Item))
                    {
                        int sold = 0;
                        _stockSold.TryGetValue(config.Item, out sold);
                        availabilities[config.Item] = -sold;
                    }
                    availabilities[config.Item] += config.InitialStock;
                }
                else
                {
                    availabilities[config.Item] = CountItemsInInventory(config.Item);
                }
            }

            return availabilities;
        }

        Dictionary<InventoryItem, float> GetPrices()
        {
            var prices = new Dictionary<InventoryItem, float>();

            foreach(StockItemConfig config in GetAvailableConfigs())
            {
                if(_isBuyingMode)
                {
                    if(!prices.ContainsKey(config.Item))
                    {
                        prices[config.Item] = config.Item.Price * GetBarterDiscount();
                    }
                    prices[config.Item] *= (1- config.BuyingDiscountPercentage / 100);
                }
                else
                {
                    prices[config.Item] = config.Item.Price * (_sellingPercentage / 100f);
                }
            }

            return prices;
        }

        float GetBarterDiscount()
        {
            BaseStats baseStats = _currentShopper.GetComponent<BaseStats>();
            float discountPercentage = baseStats.GetStat(Stat.BuyingDiscountPercentage);

            return (1 - Mathf.Min(discountPercentage, _maxBarterDiscountPercentage) / 100);
        }

        IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();
            foreach(StockItemConfig config in _stockConfig)
            {
                if(config.LevelToUnlock > shopperLevel)
                    continue;
                yield return config;
            }
        }
        #endregion
    }
}
