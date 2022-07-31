using UnityEngine;

using RPG.Core;
using RPG.Combat;
using RPG.Movement;
using System;

namespace RPG.Control
{
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(ActionScheduler))]
    public class AIController : MonoBehaviour
    {
        #region Parameters
        [SerializeField] 
        private float _chaseDistance = 5f;
        [SerializeField]
        private float _suspicionTime = 5f;
        [SerializeField]
        private float _dwellingTime = 3f;
        [SerializeField] [Range(0,1)]
        private float _patrolSpeedFraction = 0.2f;
        #endregion

        #region Cache
        [HideInInspector]
        private Fighter _fighter;
        [HideInInspector]
        private Health _health;
        [HideInInspector]
        private Mover _mover;
        [HideInInspector]
        private ActionScheduler _actionScheduler;

        [SerializeField]
        private PatrolPath _patrolPath;
        [SerializeField]
        private float _waypointTolerance = 1f;

        private GameObject _playerGameObject;
        private Vector3 _guardLocation;
        #endregion

        #region States
        private int _waypointIndex =0;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceReachedWaypoint = Mathf.Infinity;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region EngineFunctions
        private void OnValidate()
        {
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        private void Awake()
        {
            _guardLocation = transform.position; 
            UnityEngine.Assertions.Assert.IsNotNull(_patrolPath, "_patrolPath object is null");  
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
            if (!_playerGameObject)
                _playerGameObject = GameObject.FindWithTag(Enums.EnumToString<Tags>(Tags.Player));

            if (IsInAttackRangeOfPlayer() && _fighter.CanAttack(_playerGameObject))
            {
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer <= _suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceReachedWaypoint += Time.deltaTime;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
        #endregion

        #region PrivateFunctionsVoid
        private void Death()
        {
            enabled = false;
        }

        private void PatrolBehaviour()
        {
            _fighter.Cancel();

            Vector3 nextPosition = _guardLocation;
            if(_patrolPath)
            {
                if(AtWaypoint())
                {
                    if(_timeSinceReachedWaypoint > _dwellingTime)
                        CycleWaypoint();
                }
                else
                    _timeSinceReachedWaypoint = 0f;
                
                nextPosition = GetCurrentWaypoint();
            }

            _mover.StartMoveAction(nextPosition, _patrolSpeedFraction);
        }

        private void CycleWaypoint()
        {
            _waypointIndex = _patrolPath.GetNextIndex(_waypointIndex);
        }

        private void SuspicionBehavior()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_playerGameObject);
        }
        #endregion

        #region PrivateMethodsReturn
        private bool IsInAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerGameObject.transform.position);
            return distanceToPlayer <= _chaseDistance;
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(_waypointIndex);
        }
        #endregion
    }
}
