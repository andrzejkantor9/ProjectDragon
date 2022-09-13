using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "DelayCompositeEffect", menuName = "Abilities/Effects/DelayComposite")]
    public class DelayCompositeEffect : EffectStrategy
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        private float _delay = 0f;
        [SerializeField]
        private EffectStrategy[] _delayedEffects;
        [SerializeField]
        private bool _abortIfCancelled = false;
        #endregion

        #region Cache
        //[Header("CACHE")]
        //[Space(8f)]
        #endregion

        #region States
        #endregion

        #region Events & Statics
        //[Header("EVENTS")]
        //[Space(8f)]
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods & Contructors
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces & Inheritance
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayedEffect(data, finished));
        }
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        private IEnumerator DelayedEffect(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(_delay);
            if(_abortIfCancelled && data.Cancelled)
                yield break;

            foreach(EffectStrategy effect in _delayedEffects)
            {
                effect.StartEffect(data, finished);
            }
        }
        #endregion
    }
}
