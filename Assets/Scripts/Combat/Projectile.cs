using UnityEngine;

using RPG.Core;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        #region Parameters
        [SerializeField]
        private float _speed = 10f;
        [SerializeField]
        private bool _homing = true;
        [SerializeField]
        private GameObject _hitEffect;
        [SerializeField]
        private float _maxLifeTime = 10f;
        [SerializeField]
        private float _lifeAfterImpact = .5f;
        [SerializeField]
        private GameObject[] _destroyOnHit;

        private float _damage = 0;
        #endregion

        #region States
        private Health _targetHealth;
        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if(_targetHealth)
            {   
                if(_homing && !_targetHealth.IsDead)
                    transform.LookAt(GetAimLocation());

                transform.Translate(Vector3.forward * _speed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Health colliderHealth = other.GetComponent<Health>();
            if(colliderHealth && colliderHealth.IsDead)
            {
                return;
            }
            else if(colliderHealth && colliderHealth == _targetHealth)
            {
                colliderHealth.TakeDamage(_damage);
                _speed = 0f;

                if(_hitEffect)
                    Instantiate(_hitEffect, GetAimLocation(), transform.rotation);

                foreach (GameObject objectToDestory in _destroyOnHit)
                {
                    Destroy(objectToDestory);
                }
                
                Destroy(gameObject, _lifeAfterImpact);
            }
        }
        #endregion

        #region PublicMethods
        public void SetTarget(Health target, float damage)
        {
            _targetHealth = target;
            _damage = damage;

            Destroy(gameObject, _maxLifeTime);
        }
        #endregion

        #region PrivateMethods
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = _targetHealth.GetComponent<CapsuleCollider>();
            
            if(!targetCapsule) return _targetHealth.transform.position;
            return _targetHealth.transform.position + Vector3.up * targetCapsule.height / 2;
        }
        #endregion
    }
}
