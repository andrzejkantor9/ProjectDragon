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
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        #region Cache
        private Mover _mover;
        private Animator _animator;
        private ActionScheduler _actionScheduler;
        private HitPoints _hitPoints;
        private Equipment _equipment;

        [SerializeField]
        private Transform _rightHandTransformWeapon;
        [SerializeField]
        private Transform _leftHandTransformWeapon;
        [SerializeField]
        private WeaponConfig _defaultWeapon;

        private Transform _target;
        private int _attackAnimId;
        private int _stopAttackAnimId;
        #endregion

        #region States
        private float _timeSinceLastAttack = Mathf.Infinity;
        private WeaponConfig _currentWeaponConfig;
        private LazyValue<Weapon> _currentWeapon;

        private HitPoints _targetHealth;
        #endregion

        ////////////////////////////////////////////////////////////////////////

        #region EngineFunctions
        private void Awake()
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
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        private void OnEnable()
        {
            _hitPoints.OnDeath += Death;
            if(_equipment)
                _equipment.equipmentUpdated += UpdateWeapon;
        }

        private void OnDisable()
        {
            _hitPoints.OnDeath -= Death;
            if(_equipment)
                _equipment.equipmentUpdated -= UpdateWeapon;
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if(_target == null)
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
            _animator.ResetTrigger(_stopAttackAnimId);
            _actionScheduler.StartAction(this);

            _target = combatTarget.transform;
            _targetHealth = _target.GetComponent<HitPoints>();
            _targetHealth.OnDeath += OnTargetDeath;
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

        public HitPoints GetTargetHitPoints() => _targetHealth;
        #endregion

        #region Interfaces
        public void Cancel()
        {
            StopAttack();
            _target = null;
            GetComponent<Mover>().Cancel();
        }  

        public object CaptureState()
        {
            CustomLogger.Log($"gameObject: {gameObject.name}, weapon config: {_currentWeaponConfig?.name}", LogFrequency.Sporadic);
            WeaponNames weaponName = (WeaponNames) Enum.Parse(typeof(WeaponNames), _currentWeaponConfig?.name);
            CustomLogger.Log($"saved weapon: {Enums.EnumToString<WeaponNames>(weaponName)}, for character: {gameObject.name}", LogFrequency.Rare);
            return weaponName;
        }

        public void RestoreState(object state)
        {
            WeaponNames weaponName = (WeaponNames)state;
            CustomLogger.Log($"Loading weapon from enum: {Enums.EnumToString<WeaponNames>(weaponName)}, for character: {gameObject.name}", LogFrequency.Rare);
            WeaponConfig weapon = Resources.Load<WeaponConfig>(
                "Weapons" 
                + System.IO.Path.DirectorySeparatorChar 
                + Enums.EnumToString<WeaponNames>(weaponName));

            _currentWeaponConfig = weapon;
            EquipWeapon(weapon);
        }
        #endregion

        #region PrivateFunctions
        private void UpdateWeapon()
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

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(_rightHandTransformWeapon, _leftHandTransformWeapon, animator);
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(_defaultWeapon);
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_target);  
            
            if(_timeSinceLastAttack > _currentWeaponConfig.TimeBetweenAttacks)
            {
                _animator.SetTrigger(_attackAnimId);
                _timeSinceLastAttack = 0f;
            }            
        }

        private void Death()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<CapsuleCollider>().enabled = false;
            enabled = false;
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= _currentWeaponConfig.WeaponRange;
        }

        private void StopAttack()
        {
            if(_targetHealth)
                _targetHealth.OnDeath -= OnTargetDeath;

            _animator.ResetTrigger(_attackAnimId);
            _animator.SetTrigger(_stopAttackAnimId);
        }
        #endregion

        #region Events
        private void OnTargetDeath()
        {
            Cancel();
        }
        #endregion

        #region Animation Events
        private void Hit()
        {
            if(_target == null) 
                return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if(_currentWeapon.value)
                _currentWeapon.value.Hit();

            if(_currentWeaponConfig.HasProjectile)
            {
                if(!_targetHealth.IsDead)
                    _currentWeaponConfig.LaunchProjectile(_rightHandTransformWeapon, _leftHandTransformWeapon, _target.GetComponent<HitPoints>(), gameObject, damage);  
            }
            else
            {
                // _targetHealth.TakeDamage(gameObject, _currentWeapon.WeaponDamage);
                _targetHealth.TakeDamage(gameObject, damage);

                if(_targetHealth.IsDead)
                {
                    CustomLogger.Log($"{gameObject.name} stop attacking - target is dead", LogFrequency.Regular);
                    Cancel();
                }
            }
        }

        private void Shoot()
        {
            Hit();
        }
        #endregion
    }
}
