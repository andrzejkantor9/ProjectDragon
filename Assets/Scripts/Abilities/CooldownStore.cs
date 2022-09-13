using System;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        #region Config
        // [Header("CONFIG")]
        #endregion

        #region Cache
        //[Header("CACHE")]
    	//[Space(8f)]
        #endregion

        #region States
        private Dictionary<InventoryItem, float> _cooldownTimers = new Dictionary<InventoryItem, float>();
        private Dictionary<InventoryItem, float> _initialCooldownTimes = new Dictionary<InventoryItem, float>();
        #endregion

        #region Events & Statics
        //[Header("EVENTS")]
    	//[Space(8f)]
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods & Contructors
        private void Update()
        {
            var keys = new List<InventoryItem>(_cooldownTimers.Keys);
            foreach(InventoryItem ability in keys)
            {
                _cooldownTimers[ability] -= Time.deltaTime;
                if(_cooldownTimers[ability] < 0f)
                {
                    _cooldownTimers.Remove(ability);
                    _initialCooldownTimes.Remove(ability);
                }
            }
        }
        #endregion

        #region PublicMethods
        public void StartCooldown(InventoryItem ability, float cooldownTime)
        {
            _cooldownTimers[ability] = cooldownTime;
            _initialCooldownTimes[ability] = cooldownTime;
        } 

        public float GetTimeRemaining(InventoryItem ability)
        {
            if(!_cooldownTimers.ContainsKey(ability))
                return 0f;

            return _cooldownTimers[ability];
        }

        public float GetFractionRemaining(InventoryItem ability)
        {
            if(!ability || !_cooldownTimers.ContainsKey(ability))
                return 0f;

            return _cooldownTimers[ability] / _initialCooldownTimes[ability];
        }
        #endregion

        #region Interfaces & Inheritance
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        #endregion
    }
}
