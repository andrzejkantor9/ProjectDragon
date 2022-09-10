using System;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        #region Config
        // [Header("CONFIG")]
        #endregion

        #region Cache
        // [Header("CACHE")]
        //[Space(8f)]
        private Shop _activeShop;
        #endregion

        #region States
        #endregion

        #region Events
        // [Header("EVENTS")]
        //[Space(8f)]
        public event Action onActiveShopChange;
        #endregion

        #region Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        #endregion

        #region PublicMethods
        public Shop ActiveShop => _activeShop;

        public void SetActiveShop(Shop shop)
        {
            if(_activeShop != null)
            {
                _activeShop.SetShopper(null);
            }
            _activeShop = shop;

            if(_activeShop != null)
            {
                _activeShop.SetShopper(this);
            }

            if(onActiveShopChange != null)
                onActiveShopChange();
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
