using System;

using UnityEngine;

using GameDevTV.Inventories;

using RPG.Debug;
using RPG.Saving;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable, IItemStore
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        private float _startingBalance = 400f;
        #endregion

        #region Cache
        //[Header("CACHE")]
    	//[Space(8f)]
        #endregion

        #region States
        private float _balance;
        #endregion

        #region Events
        //[Header("EVENTS")]
    	//[Space(8f)]
        public event Action onChange;
        #endregion

        #region Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            _balance = _startingBalance;    
            CustomLogger.Log($"Balance: {_balance}", LogFrequency.Sporadic);
        }
        #endregion

        #region PublicMethods
        public float Balance => _balance;

        public void UpdateBalance(float amount)
        {
            _balance += amount;
            CustomLogger.Log($"Balance: {_balance}", LogFrequency.Rare);

            if(onChange != null)
                onChange();
        }
        #endregion

        #region Interfaces
        public object CaptureState()
        {
            return _balance;
        }

        public void RestoreState(object state)
        {
            _balance = (float) state;
        }

        public int AddItems(InventoryItem item, int number)
        {
            if(item is CurrencyItem)
            {
                UpdateBalance(item.Price * number);
                return number;
            }
            return 0;
        }
        #endregion

        #region Events
        #endregion

        #region StaticMethods
        #endregion

        #region PrivateMethods
        #endregion
    }
}
