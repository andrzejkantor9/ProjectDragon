using UnityEngine;

using RPG.Core;
using RPG.Shops;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        #region Config
        // [Header("CONFIG")]
        #endregion

        #region Cache
        // [Header("CACHE")]
        //[Space(8f)]
        #endregion

        #region States
        private Shopper _shopper;
        private Shop _currentShop;
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
        private void Awake()
        {
            _shopper = GameManager.PlayerGameObject.GetComponent<Shopper>();
            if(_shopper)
                _shopper.onActiveShopChange += ShopChanged;

            ShopChanged();
        }
        
        private void OnDestroy()
        {
            if(_shopper)
                _shopper.onActiveShopChange -= ShopChanged;
        }
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces
        #endregion

        #region StaticMethods
        #endregion

        #region Events
        private void ShopChanged()
        {
            _currentShop = _shopper.ActiveShop;
            gameObject.SetActive(_currentShop != null);
        }
        #endregion

        #region PrivateMethods
        #endregion
    }
}
