using System;
using System.Collections.Generic;

using UnityEngine;

using GameDevTV.Utils;
using GameDevTV.Inventories;

using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Stats;
using RPG.Debug;
using RPG.Saving;

namespace RPG.Combat
{
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(HitPoints))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Fighter : MonoBehaviour, IAction
    {
        #region Config
        [SerializeField]
        float _autoAttackRange = 4f;
        #endregion

        #region Cache
        Mover _mover;
        Animator _animator;
        ActionScheduler _actionScheduler;
        HitPoints _hitPoints;
        Equipment _equipment;

        [SerializeField]
        Transform _rightHandTransformWeapon;
        [SerializeField]
        Transform _leftHandTransformWeapon;
        [SerializeField]
        WeaponConfig _defaultWeapon;

        Transform _target;
        int _attackAnimId;
        int _stopAttackAnimId;
        #endregion

        #region States
        float _timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig _currentWeaponConfig;
        LazyValue<Weapon> _currentWeapon;

        HitPoints _targetHitPoints;
        #endregion

        ////////////////////////////////////////////////////////////////////////

        #region EngineFunctions
        void Awake()
        {
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();            
            _actionScheduler = GetComponent<ActionScheduler>(); 
            _hitPoints = GetComponent<HitPoints>();
            _equipment = GetComponent<Equipment>();

            _attackAnimId = Animator.StringToHash("Attack");
            _stopAttackAnimId = Animator.StringToHash("StopAttack");

            UnityEngine.Assertions.Assert.IsNotNull(_defaultWeapon, "_defaultWeapon is null");

            _currentWeaponConfig = _defaultWeapon;
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);

            _hitPoints.onRespawn += Respawn;
        }

        void Start()
        {
            _currentWeapon.ForceInit();
        }

        void OnEnable()
        {
            _hitPoints.OnDeath += Death;
            if(_equipment)
                _equipment.equipmentUpdated += UpdateWeapon;
        }

        void OnDisable()
        {
            _hitPoints.OnDeath -= Death;
            if(_equipment)
                _equipment.equipmentUpdated -= UpdateWeapon;
        }

        void OnDestroy() 
        {
            _hitPoints.onRespawn -= Respawn;
        }

        void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            if(!_target && GetComponent<Mover>().IsStopped())
            {
                _target = FindNewTargetInRange();
                if(_target)
                {
                    _targetHitPoints = _target.GetComponent<HitPoints>();
                }
            }
            if(!_target)
                    return;

            _actionScheduler.StartAction(this);

            if (!GetIsInRange(_target.transform) && !_target.GetComponent<HitPoints>().IsDead)
            {
                _mover.MoveTo(_target.position, 1f);
            }
            else
            {
                _mover.Cancel();
                AttackBehaviour();
            }

        }
        #endregion

        #region PublicMethods
        public void Attack(GameObject combatTarget)
        {
            CustomLogger.Log($"Attack: {combatTarget.name}, by: {gameObject.name}", LogFrequency.MostFrames);
            _animator.ResetTrigger(_stopAttackAnimId);
            _actionScheduler.StartAction(this);

            _target = combatTarget.transform;
            _targetHitPoints = _target.GetComponent<HitPoints>();
            _targetHitPoints.OnDeath += OnTargetDeath;
        }
        
        public bool CanAttack(GameObject combatTarget)
        {
            if(!combatTarget)
                return false;
            if(!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform))
                return false;
            
            HitPoints targetHitpoints = combatTarget.GetComponent<HitPoints>();
            return targetHitpoints && !targetHitpoints.IsDead;
        }

        public Transform GetHandTransform(bool isRightHand)
        {
            if(isRightHand)
                return _rightHandTransformWeapon;
            else
                return _leftHandTransformWeapon;
        }

        public HitPoints GetTargetHitPoints() => _targetHitPoints;
        #endregion

        #region Interfaces
        public void Cancel()
        {
            StopAttack();
            _target = null;
            GetComponent<Mover>().Cancel();
        }
        #endregion

        #region PrivateFunctions
        void UpdateWeapon()
        {
            WeaponConfig weaponConfig = _equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;   
            if(weaponConfig == null)
                EquipWeapon(_defaultWeapon);
            else
                EquipWeapon(weaponConfig);
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            CustomLogger.Log($"equip weapon: {weapon?.name}, for character: {gameObject.name}", LogFrequency.Regular);
            _currentWeaponConfig = weapon;

            _currentWeapon.value = AttachWeapon(weapon);
        }

        Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(_rightHandTransformWeapon, _leftHandTransformWeapon, animator);
        }

        Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(_defaultWeapon);
        }

        void AttackBehaviour()
        {
            transform.LookAt(_target);  
            
            if(_timeSinceLastAttack > _currentWeaponConfig.TimeBetweenAttacks)
            {
                _animator.SetTrigger(_attackAnimId);
                _timeSinceLastAttack = 0f;
            }            
        }

        void Death()
        {
            SetDead(true);
        }

        void SetDead(bool isDead)
        {
            GetComponent<Rigidbody>().isKinematic = isDead;
            GetComponent<CapsuleCollider>().enabled = !isDead;
            enabled = !isDead;
        }

        void Respawn()
        {
            SetDead(false);
        }

        bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= _currentWeaponConfig.WeaponRange;
        }

        void StopAttack()
        {
            if(_targetHitPoints)
                _targetHitPoints.OnDeath -= OnTargetDeath;

            _animator.ResetTrigger(_attackAnimId);
            _animator.SetTrigger(_stopAttackAnimId);
        }
        #endregion

        #region Events
        void OnTargetDeath()
        {
            Cancel();
        }
        #endregion

        #region Animation Events
        void Hit()
        {
            if(_targetHitPoints == null) 
                return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            var targetBaseStats = _targetHitPoints.GetComponent<BaseStats>();
            if(targetBaseStats)
            {
                float defence = targetBaseStats.GetStat(Stat.Defence);
                damage /= 1f+ defence / damage;
            }

            if(_currentWeapon.value)
                _currentWeapon.value.Hit();

            if(_currentWeaponConfig.HasProjectile)
            {
                if(!_targetHitPoints.IsDead)
                    _currentWeaponConfig.LaunchProjectile(_rightHandTransformWeapon, _leftHandTransformWeapon, _target.GetComponent<HitPoints>(), gameObject, damage);  
            }
            else
            {
                // _targetHealth.TakeDamage(gameObject, _currentWeapon.WeaponDamage);
                _targetHitPoints.TakeDamage(gameObject, damage);

                if(_targetHitPoints.IsDead)
                {
                    CustomLogger.Log($"{gameObject.name} stop attacking - target is dead", LogFrequency.Regular);
                    Cancel();
                }
            }
        }

        void Shoot()
        {
            Hit();
        }
        #endregion

        #region PrivateMethods
        Transform FindNewTargetInRange()
        {
            Transform bestTarget = null;
            float bestDistance = Mathf.Infinity;
            
            foreach(Transform candidate in FindAllTargetsInRange())
            {
                float candidateDistance = Vector3.Distance(transform.position, candidate.transform.position);
                if(candidateDistance < bestDistance)
                {
                    bestTarget = candidate;
                    bestDistance = candidateDistance;
                }
            }

            Debug.CustomLogger.Log($"character: {gameObject.name}, best target: {bestTarget?.name}",
                Debug.LogFrequency.EveryFrame);
            return bestTarget;
        }

        private IEnumerable<Transform> FindAllTargetsInRange()
        {
            if(!gameObject.CompareTag(Enums.EnumToString<Tags>(Tags.Player)))
                yield break;

            RaycastHit[] raycastHits = 
                Physics.SphereCastAll(transform.position, _autoAttackRange, Vector3.up);

            foreach(RaycastHit hit in raycastHits)
            {
                var hitPoints = hit.transform.GetComponent<HitPoints>();
                var combatTarget = hit.transform.GetComponent<CombatTarget>();
                if(hitPoints == null || hitPoints.IsDead || hitPoints.gameObject == gameObject
                    || !combatTarget || !combatTarget.enabled)
                    continue;
                yield return hit.transform;
            }
        }
        #endregion
    }
}
