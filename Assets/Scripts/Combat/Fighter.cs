using System;

using UnityEngine;

using RPG.Movement;
using RPG.Core;

//TODO on target death event wycieki możliwe?
namespace RPG.Combat
{
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        #region Cache
        [HideInInspector]
        private Mover _mover;
        [HideInInspector]
        private Animator _animator;
        [HideInInspector]
        private ActionScheduler _actionScheduler;
        [HideInInspector]
        private Health _health;

        [SerializeField]
        private Transform _rightHandTransformWeapon;
        [SerializeField]
        private Transform _leftHandTransformWeapon;
        [SerializeField]
        private WeaponNames _defaultWeapon = WeaponNames.Unarmed;

        private Transform _target;
        private int _attackAnimId;
        private int _stopAttackAnimId;
        #endregion

        #region States
        private float _timeSinceLastAttack = Mathf.Infinity;
        private Weapon _currentWeapon;
        private Health _targetHealth;
        #endregion

        ////////////////////////////////////////////////////////////////////////

        #region EngineFunctions
        private void OnValidate()
        {
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();            
            _actionScheduler = GetComponent<ActionScheduler>(); 
            _health = GetComponent<Health>();
        }

        private void Awake()
        {
            _attackAnimId = Animator.StringToHash("Attack");
            _stopAttackAnimId = Animator.StringToHash("StopAttack");

            // UnityEngine.Assertions.Assert.IsNotNull(_weaponPrefab, "weapon prefab null");
            // UnityEngine.Assertions.Assert.IsNotNull(_handTransformWeapon, "_handTransformWeapon null");
        }

        private void Start()
        {
            if(_currentWeapon == null)
            {
                Weapon weapon = Resources.Load<Weapon>(Enums.EnumToString<WeaponNames>(_defaultWeapon));
                EquipWeapon(weapon);

                Logger.Log($"set weapon (start): {Enums.EnumToString<WeaponNames>(_defaultWeapon)}, for character: {gameObject.name}");
            }
        }

        private void OnEnable()
        {
            _health.OnDeath += Death;
        }

        private void OnDisable()
        {
            _health.OnDeath -= Death;
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if(_target == null)
                return;

            _actionScheduler.StartAction(this);

            if (!GetIsInRange() && !_target.GetComponent<Health>().IsDead)
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

        #region PublicFunctions
        public void Attack(GameObject combatTarget)
        {
            _animator.ResetTrigger(_stopAttackAnimId);
            _actionScheduler.StartAction(this);

            _target = combatTarget.transform;
            _targetHealth = _target.GetComponent<Health>();
            _targetHealth.OnDeath += OnTargetDeath;
        }
        public bool CanAttack(GameObject combatTarget)
        {
            return combatTarget != null && !combatTarget.GetComponent<Health>().IsDead;
        }
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
            WeaponNames weaponName = (WeaponNames) Enum.Parse(typeof(WeaponNames), _currentWeapon.name);
            Logger.Log($"saved weapon: {Enums.EnumToString<WeaponNames>(weaponName)}, for character: {gameObject.name}");
            return weaponName;
        }

        public void RestoreState(object state)
        {
            WeaponNames weaponName = (WeaponNames)state;
            Weapon weapon = Resources.Load<Weapon>(Enums.EnumToString<WeaponNames>(weaponName));
            Logger.Log($"loaded weapon: {Enums.EnumToString<WeaponNames>(weaponName)}, for character: {gameObject.name}");

            _currentWeapon = weapon;
            EquipWeapon(weapon);
        }
        #endregion

        #region PrivateFunctions
        public void EquipWeapon(Weapon weapon)
        {
            Logger.Log($"equip weapon: {weapon.name}, for character: {gameObject.name}");
            _currentWeapon = weapon;

            Animator animator = GetComponent<Animator>();
            weapon.Spawn(_rightHandTransformWeapon, _leftHandTransformWeapon, animator);
        }
        private void AttackBehaviour()
        {
            transform.LookAt(_target);  
            
            if(_timeSinceLastAttack > _currentWeapon.TimeBetweenAttacks)
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

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.position) <= _currentWeapon.WeaponRange;
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

            if(_currentWeapon.HasProjectile)
            {
                if(!_targetHealth.IsDead)
                    _currentWeapon.LaunchProjectile(_rightHandTransformWeapon, _leftHandTransformWeapon, _target.GetComponent<Health>());  
            }
            else
            {
                _targetHealth.TakeDamage(_currentWeapon.WeaponDamage);
                if(_targetHealth.IsDead)
                {
                    Logger.Log($"{gameObject.name} stop attacking - target is dead");
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
