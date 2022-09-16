using UnityEngine;
using UnityEngine.Events;

using RPG.Attributes;
using RPG.Debug;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        #region Parameters
        [SerializeField]
        float _speed = 10f;
        [SerializeField]
        bool _homing = true;
        [SerializeField]
        GameObject _hitEffect;
        [SerializeField]
        float _maxLifeTime = 10f;
        [SerializeField]
        float _lifeAfterImpact = .5f;
        [SerializeField]
        GameObject[] _destroyOnHit;

        private float _damage = 0;
        #endregion

        #region States
        HitPoints _targetHitPoints;
        GameObject _instigator;
        Vector3 _targetPoint;
        #endregion

        #region Events
        [SerializeField]
        UnityEvent OnProjectileHit;
        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if(_homing && _targetHitPoints && !_targetHitPoints.IsDead)
            {
                transform.LookAt(GetAimLocation());
            }
            else
            {
                _targetHitPoints = null;
            }

            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other)
        {
            HitPoints colliderHitPoints = other.GetComponent<HitPoints>();
            if(!colliderHitPoints || colliderHitPoints.IsDead || other.gameObject == _instigator)
                return;

            CustomLogger.Log($"_homing: {_homing}, colliderHealth: {colliderHitPoints?.gameObject.name}" +
                $", target health: {_targetHitPoints?.gameObject.name}", LogFrequency.Regular);
            if(_homing && colliderHitPoints == _targetHitPoints)
            {
                ProjectileHit(colliderHitPoints);
            }
            else if(!_homing)
            {
                ProjectileHit(colliderHitPoints);
            }
        }
        #endregion

        #region PublicMethods
        public void SetTarget(HitPoints target, GameObject instigator, float damage, bool homing = true)
        {
            SetTarget(instigator, damage, target, homing: homing);
        }

        public void SetTarget(Vector3 targetPoint, GameObject instigator, float damage, bool homing = true)
        {
            SetTarget(instigator, damage, targetPoint: targetPoint, homing: homing);
        }

        public void SetTarget(
            GameObject instigator, float damage, HitPoints target = null,
            Vector3 targetPoint = default, bool homing = true)
        {
            _homing = homing;
            _targetHitPoints = target;
            _targetPoint = targetPoint;
            _damage = damage;
            _instigator = instigator;

            Destroy(gameObject, _maxLifeTime);
        }
        #endregion

        #region PrivateMethods
        Vector3 GetAimLocation()
        {   
            if(!_targetHitPoints)
                return _targetPoint;

            CapsuleCollider targetCapsule = _targetHitPoints.GetComponent<CapsuleCollider>();
            
            if(!targetCapsule) 
                return _targetHitPoints.transform.position;

            return _targetHitPoints.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        void ProjectileHit(HitPoints colliderHealth)
        {
            CustomLogger.Log($"projectile hit: {colliderHealth.gameObject.name}", LogFrequency.Regular);
            colliderHealth.TakeDamage(_instigator, _damage);
            _speed = 0f;

            if (_hitEffect)
                Instantiate(_hitEffect, transform.position, transform.rotation);

            foreach (GameObject objectToDestory in _destroyOnHit)
            {
                Destroy(objectToDestory);
            }

            OnProjectileHit?.Invoke();
            Destroy(gameObject, _lifeAfterImpact);
        }
        #endregion
    }
}
