using System.Collections.Generic;

using UnityEngine;

using GameDevTV.Inventories;
using GameDevTV.Utils;

using RPG.Debug;
using RPG.Attributes;
using RPG.Core;

//Program Files (x86)\Unity\<engine version>\Editor\Data\Resources\ScriptTemplates
namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Abilities/Ability")]
    public class Ability : ActionItem
    {
        #region Config
        //[Header("CONFIG")]
        [SerializeField]
        private TargetingStrategy _targetingStrategy;
        [SerializeField]
        private FilterStrategy _filterStrategy;
        [SerializeField]
        private EffectStrategy[] _effectStrategies;
        [SerializeField]
        private float _cooldownTime = 5f;
        [SerializeField]
        private float _manaCost = 0f;
        #endregion

        #region Cache
        //[Header("CACHE")]
    	//[Space(8f)]
        #endregion

        #region States
        #endregion

        #region Events
        //[Header("EVENTS")]
    	//[Space(8f)]
        #endregion

        #region Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces & Inheritance
        public override bool Use(GameObject user)
        {
            Mana mana = user.GetComponent<Mana>();
            if(mana.ManaPoints.value < _manaCost)
                return false;

            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            if(cooldownStore && cooldownStore.GetTimeRemaining(this) > 0f)
                return false;

            AbilityData data = new AbilityData(user);

            ActionScheduler actionScheduler = user.GetComponent<ActionScheduler>();
            actionScheduler.StartAction(data);

            _targetingStrategy.StartTargeting(
                data,
                () => 
                {
                    TargetAquired(data);
                });

            return true;
        }
        #endregion

        #region Events
        #endregion
	
        #region StaticMethods
        #endregion

        #region PrivateMethods
        private void TargetAquired(AbilityData data)
        {
            if(data.Cancelled)
                return;

            Mana mana = data.User.GetComponent<Mana>();
            if(!mana.UseMana(_manaCost))
                return;

            var cooldownStore = data.User.GetComponent<CooldownStore>();
            if(cooldownStore)
                cooldownStore.StartCooldown(this, _cooldownTime);

            if(_filterStrategy)
            {
                string targetList = "targets aquired: ";
                data.SetTargets(_filterStrategy.Filter(data.Targets));

#if UNITY_DEVElOPMENT || UNITY_EDITOR
                foreach(GameObject target in data.Targets)
                {
                    targetList += target.name + ", ";
                }
                CustomLogger.Log(targetList, LogFrequency.Regular);
#endif
            }
            foreach(EffectStrategy effectStrategy in _effectStrategies)
            {
                effectStrategy.StartEffect(data, EffecFinished);
            }
        }

        private void EffecFinished()
        {
        }
        #endregion
    }
}
