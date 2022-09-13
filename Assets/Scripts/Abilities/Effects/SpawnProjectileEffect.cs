using System;

using UnityEngine;

using RPG.Combat;
using RPG.Attributes;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "SpawnProjectileEffect", menuName = "Abilities/Effects/SpawnProjectile")]
    public class SpawnProjectileEffect : EffectStrategy
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        Projectile _projectileToSpawn;
        [SerializeField]
        float _damage;
        [SerializeField]
        bool _isRightHanded = true;
        [SerializeField]
        bool _useTargetPoint = true;
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
            var fighter = data.User.GetComponent<Fighter>();
            Vector3 spawnPosition = fighter.GetHandTransform(_isRightHanded).position;

            if(_useTargetPoint)
            {
                SpawnProjectileForTargetPoint(data, spawnPosition);
            }
            else
            {
                SpawnProjectilesForTargets(data, spawnPosition);
            }

            finished();
        }
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        void SpawnProjectileForTargetPoint(AbilityData data, Vector3 spawnPosition)
        {
            Projectile projectile = Instantiate(_projectileToSpawn);
            projectile.transform.position = spawnPosition;
            projectile.SetTarget(data.TargetedPoint, data.User, _damage, false);
        }

        void SpawnProjectilesForTargets(AbilityData data, Vector3 spawnPosition)
        {
            foreach (GameObject target in data.Targets)
            {
                var hitPoints = target.GetComponent<HitPoints>();
                if (hitPoints)
                {
                    Projectile projectile = Instantiate(_projectileToSpawn);
                    projectile.transform.position = spawnPosition;
                    projectile.SetTarget(hitPoints, data.User, _damage, false);
                }
            }
        }
        #endregion
    }
}
