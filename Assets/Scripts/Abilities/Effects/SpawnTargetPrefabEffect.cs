using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "SpawnTargetPrefabEffect", menuName = "Abilities/Effects/SpawnTargetPrefab")]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        #region Config
        //[Header("CONFIG")]
        #endregion

        #region Cache
        [Header("CACHE")]
        //[Space(8f)]
        [SerializeField]
        private Transform _prefabToSpawn;
        [SerializeField]
        private float _destroyDelay = -1f;
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
            data.StartCoroutine(Effect(data, finished));
        }
        #endregion

        #region Events
        #endregion

        #region StaticMethods
        #endregion

        #region PrivateMethods
        private IEnumerator Effect(AbilityData data, Action finished)
        {
            Transform instance = Instantiate(_prefabToSpawn);
            instance.position = data.TargetedPoint;
            if(_destroyDelay > 0)
            {
                yield return new WaitForSeconds(_destroyDelay);
                Destroy(instance.gameObject);
            }
            finished();
        }
        #endregion
    }
}
