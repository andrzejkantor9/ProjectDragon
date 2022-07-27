using UnityEngine;

using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ActionScheduler))]
    public class Fighter : MonoBehaviour, IAction
    {
        #region Parameters
        [SerializeField]
        private float _weaponRange = 2f;
        [SerializeField]
        private float _timeBetweenAttacks = 2f;
        [SerializeField]
        private float _weaponDamage = 5f;
        #endregion

        #region Cache
        [HideInInspector]
        private Mover _mover;
        [HideInInspector]
        private Animator _animator;
        [HideInInspector]
        private ActionScheduler _actionScheduler;

        private Transform _target;
        private int _attackAnimId;
        private int _stopAttackAnimId;
        #endregion

        #region States
        private float _timeSinceLastAttack = 0f;
        #endregion

        ////////////////////////////////////////////////////////////////////////

        #region EngineFunctionality
        private void OnValidate()
        {
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();            
            _actionScheduler = GetComponent<ActionScheduler>(); 
        }

        private void Awake()
        {
            _attackAnimId = Animator.StringToHash("Attack");
            _stopAttackAnimId = Animator.StringToHash("StopAttack");
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if(_target == null)
                return;

            _actionScheduler.StartAction(this);

            if (!GetIsInRange() && !_target.GetComponent<Health>().IsDead)
            {
                _mover.MoveTo(_target.position);
            }
            else
            {
                _mover.Cancel();
                AttackBehaviour();
            }

        }

        private void AttackBehaviour()
        {
            transform.LookAt(_target);  
            
            if(_timeSinceLastAttack > _timeBetweenAttacks)
            {
                _animator.SetTrigger(_attackAnimId);
                _timeSinceLastAttack = 0f;
            }            
        }
        #endregion

        #region PublicFunctionality
        public void Attack(CombatTarget combatTarget)
        {
            _animator.ResetTrigger(_stopAttackAnimId);
            _actionScheduler.StartAction(this);
            _target = combatTarget.transform;
        }
        public bool CanAttack(CombatTarget combatTarget)
        {
            return combatTarget != null && !combatTarget.GetComponent<Health>().IsDead;
        }
        #endregion

        #region Interfaces
        public void Cancel()
        {
            StopAttack();
            _target = null;
        }
        #endregion

        #region PrivateFunctionality
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.position) <= _weaponRange;
        }

        private void StopAttack()
        {
            _animator.ResetTrigger(_attackAnimId);
            _animator.SetTrigger(_stopAttackAnimId);
        }

        //animation event
        private void Hit()
        {
            if(_target == null) return;

            Health targetHealth = _target.GetComponent<Health>();
            targetHealth.TakeDamage(_weaponDamage);
            if(targetHealth.IsDead)
            {
                Cancel();
            }
        }
        #endregion
    }
}
