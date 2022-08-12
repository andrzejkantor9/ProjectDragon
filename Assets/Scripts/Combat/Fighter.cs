using System;

using UnityEngine;

using GameDevTV.Utils;

using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;

//TODO on target death event wycieki możliwe?
namespace RPG.Combat
{
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(HitPoints))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        #region Cache
        [HideInInspector]
        private Mover _mover;
        [HideInInspector]
        private Animator _animator;
        [HideInInspector]
        private ActionScheduler _actionScheduler;
        [HideInInspector]
        private HitPoints _hitPoints;

        [SerializeField]
        private Transform _rightHandTransformWeapon;
        [SerializeField]
        private Transform _leftHandTransformWeapon;
        [SerializeField]
        private Weapon _defaultWeapon;

        private Transform _target;
        private int _attackAnimId;
        private int _stopAttackAnimId;
        #endregion

        #region States
        private float _timeSinceLastAttack = Mathf.Infinity;
        private LazyValue<Weapon> _currentWeapon;

        private HitPoints _targetHealth;
        #endregion

        ////////////////////////////////////////////////////////////////////////

        #region EngineFunctions
        private void OnValidate()
        {
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();            
            _actionScheduler = GetComponent<ActionScheduler>(); 
            _hitPoints = GetComponent<HitPoints>();
        }

        private void Awake()
        {
            _attackAnimId = Animator.StringToHash("Attack");
            _stopAttackAnimId = Animator.StringToHash("StopAttack");

            UnityEngine.Assertions.Assert.IsNotNull(_defaultWeapon, "_defaultWeapon is null");

            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private void Start()
        {
            // if(_currentWeapon == null)
            // {
            //     Weapon weapon = Resources.Load<Weapon>(Enums.EnumToString<WeaponNames>(_defaultWeaponName));
            //     EquipWeapon(weapon);

            //     Logger.Log($"set weapon (start): {Enums.EnumToString<WeaponNames>(_defaultWeaponName)}, for character: {gameObject.name}", LogFrequency.Rare);
            // }

            _currentWeapon.ForceInit();
        }

        private void OnEnable()
        {
            _hitPoints.OnDeath += Death;
        }

        private void OnDisable()
        {
            _hitPoints.OnDeath -= Death;
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if(_target == null)
                return;

            _actionScheduler.StartAction(this);

            if (!GetIsInRange() && !_target.GetComponent<HitPoints>().IsDead)
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
            return combatTarget != null && !combatTarget.GetComponent<HitPoints>().IsDead;
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

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return _currentWeapon.value.WeaponDamage;
            }
        }    

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
                yield return _currentWeapon.value.PercentageModifier;
        }    

        public object CaptureState()
        {
            WeaponNames weaponName = (WeaponNames) Enum.Parse(typeof(WeaponNames), _currentWeapon.value.name);
            Logger.Log($"saved weapon: {Enums.EnumToString<WeaponNames>(weaponName)}, for character: {gameObject.name}", LogFrequency.Rare);
            return weaponName;
        }

        public void RestoreState(object state)
        {
            WeaponNames weaponName = (WeaponNames)state;
            Weapon weapon = Resources.Load<Weapon>(Enums.EnumToString<WeaponNames>(weaponName));
            Logger.Log($"loaded weapon: {Enums.EnumToString<WeaponNames>(weaponName)}, for character: {gameObject.name}", LogFrequency.Rare);

            _currentWeapon.value = weapon;
            EquipWeapon(weapon);
        }
        #endregion

        #region PrivateFunctions
        public void EquipWeapon(Weapon weapon)
        {
            Logger.Log($"equip weapon: {weapon.name}, for character: {gameObject.name}", LogFrequency.Regular);
            _currentWeapon.value = weapon;

            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(_rightHandTransformWeapon, _leftHandTransformWeapon, animator);
        }

        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(_defaultWeapon);
            return _defaultWeapon;
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_target);  
            
            if(_timeSinceLastAttack > _currentWeapon.value.TimeBetweenAttacks)
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
            return Vector3.Distance(transform.position, _target.position) <= _currentWeapon.value.WeaponRange;
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
            if(_currentWeapon.value.HasProjectile)
            {
                if(!_targetHealth.IsDead)
                    _currentWeapon.value.LaunchProjectile(_rightHandTransformWeapon, _leftHandTransformWeapon, _target.GetComponent<HitPoints>(), gameObject, damage);  
            }
            else
            {
                // _targetHealth.TakeDamage(gameObject, _currentWeapon.WeaponDamage);
                _targetHealth.TakeDamage(gameObject, damage);

                if(_targetHealth.IsDead)
                {
                    Logger.Log($"{gameObject.name} stop attacking - target is dead", LogFrequency.Regular);
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
