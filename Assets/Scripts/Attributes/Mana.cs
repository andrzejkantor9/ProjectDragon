using UnityEngine;

using GameDevTV.Utils;

using RPG.Stats;
using RPG.Saving;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour, ISaveable
    {
        #region Config
        // [Header("CONFIG")]
        #endregion

        #region Cache
        //[Header("CACHE")]
    	//[Space(8f)]
        #endregion

        #region States
        public LazyValue<float> ManaPoints {get; private set;}
        #endregion

        #region Events & Statics
        //[Header("EVENTS")]
    	//[Space(8f)]
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods & Contructors
        private void Awake()
        {
            ManaPoints = new LazyValue<float>(GetMaxMana);
        }

        private void Update()
        {
            if(ManaPoints.value < GetMaxMana())
            {
                ManaPoints.value += GetRegenRate() * Time.deltaTime;
                if(ManaPoints.value > GetMaxMana())
                {
                    ManaPoints.value = GetMaxMana();
                }
            }
        }
        #endregion

        #region PublicMethods
        public float GetMaxMana()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Mana);
        }

        public float GetRegenRate()
        {
            return GetComponent<BaseStats>().GetStat(Stat.ManaRegen);
        }

        public bool UseMana(float manaToUse)
        {
            if(manaToUse > ManaPoints.value)
                return false;

            ManaPoints.value -= manaToUse;
            return true;
        }
        #endregion

        #region Interfaces & Inheritance
        public object CaptureState()
        {
            return ManaPoints.value;
        }

        public void RestoreState(object state)
        {
            ManaPoints.value = (float) state;
        }
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        #endregion
    }
}
