using System;

using UnityEngine;
using UnityEngine.AI;

using GameDevTV.Utils;

using RPG.Core;
using RPG.Combat;
using RPG.Movement;
using RPG.Attributes;

namespace RPG.Control
{
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(HitPoints))]
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
        private float _aggroCooldownTime = 5f;
        [SerializeField]
        private float _dwellingTime = 3f;
        [SerializeField] [Range(0,1)]
        private float _patrolSpeedFraction = 0.2f;
        [SerializeField]
        private float _aggrevateRadius = 5f;
        #endregion

        #region Cache
        [HideInInspector]
        private Fighter _fighter;
        [HideInInspector]
        private HitPoints _health;
        [HideInInspector]
        private Mover _mover;
        [HideInInspector]
        private ActionScheduler _actionScheduler;

        [SerializeField]
        private PatrolPath _patrolPath;
        [SerializeField]
        private float _waypointTolerance = 1f;

        private GameObject _playerGameObject;
        private LazyValue<Vector3> _guardLocation;
        #endregion

        #region States
        private int _waypointIndex =0;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceReachedWaypoint = Mathf.Infinity;
        private float _timeSinceAggrevated = Mathf.Infinity;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region EngineFunctions
        private void OnValidate()
        {
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<HitPoints>();
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        private void Awake()
        {
            // UnityEngine.Assertions.Assert.IsNotNull(_patrolPath, "_patrolPath object is null");  

            _playerGameObject = GameManager.PlayerGameObject();

            _guardLocation = new LazyValue<Vector3>(GetGuardPosition);

            _guardLocation.ForceInit();
            _health.onRespawn += Respawn;
        }

        private void OnEnable()
        {
            _health.OnDeath += Death;
        }

        private void OnDisable()
        {
            _health.OnDeath -= Death;
        }

        private void OnDestroy() 
        {
            _health.onRespawn -= Respawn;
        }

        private void Update()
        {
            if (IsAggrevated() && _fighter.CanAttack(_playerGameObject))
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
            _timeSinceAggrevated += Time.deltaTime;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
        #endregion

        #region PublicMethods
        public void Aggrevate()
        {
            _timeSinceAggrevated = 0f;
        }
        
        public void Reset()
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.Warp(_guardLocation.value);

            _waypointIndex =0;
            _timeSinceLastSawPlayer = Mathf.Infinity;
            _timeSinceReachedWaypoint = Mathf.Infinity;
            _timeSinceAggrevated = Mathf.Infinity;
        }
        #endregion

        #region PrivateMethodsVoid
        private void Death()
        {
            enabled = false;
        }

        private void Respawn()
        {
            enabled = true;
        }

        private void PatrolBehaviour()
        {
            _fighter.Cancel();

            Vector3 nextPosition = _guardLocation.value;
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

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void SuspicionBehavior()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_playerGameObject);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, _aggrevateRadius, Vector3.up, 0f);
            foreach (RaycastHit hit in hits)
            {
                AIController aIController = hit.collider.GetComponent<AIController>();
                if(aIController)
                    aIController.Aggrevate();
            }
        }
        #endregion

        #region PrivateMethodsReturn
        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerGameObject.transform.position);
            return distanceToPlayer <= _chaseDistance || _timeSinceAggrevated < _aggroCooldownTime;
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
