using System;
using System.Collections.Generic;

using UnityEngine;

using RPG.Attributes;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "HitPointsEffect", menuName = "Abilities/Effects/HitPoints")]
    public class HitPointsEffect : EffectStrategy
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        private float _hitPointsChange;
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
        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach(GameObject target in data.Targets)
            {
                var hitPoints = target.GetComponent<HitPoints>();
                if(hitPoints)
                {
                    if(_hitPointsChange < 0)
                    {
                        hitPoints.TakeDamage(data.User, -_hitPointsChange);
                    }
                    else
                    {
                        hitPoints.Heal(_hitPointsChange);
                    }
                }
            }

            finished();
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
