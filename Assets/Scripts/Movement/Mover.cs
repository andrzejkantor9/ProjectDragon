using UnityEngine;
using UnityEngine.AI;

using RPG.Core;

//MAKE move to clicked area component
    //decouple animations
    //inject navmesh & make it a pure c# class
    //inject action on which it should decide if click to move should apply
    //make any point input work
        //pass input action instead of doing it here
    //remember it should work out of the box, when adding component - as little extra work as possible

//TODO use cinemachine instead of camera
namespace RPG.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Health))]
    public class Mover : MonoBehaviour, IAction
    {
        #region Parameters
        [SerializeField]
        private float _maxSpeed = 6f;
        #endregion

        #region Cache
        [HideInInspector]
        private NavMeshAgent _navMeshAgent;
        [HideInInspector]
        private Animator _animator;
        [HideInInspector]
        private ActionScheduler _actionScheduler;
        [HideInInspector]
        private Health _health;

        private int _ForwardSpeedAnimId;
        #endregion

        ///////////////////////////////////////////////////////

        #region EngineFunctionality
        private void OnValidate() 
        {        
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _health = GetComponent<Health>();
        }

        private void Awake()
        {
            _ForwardSpeedAnimId = Animator.StringToHash("ForwardSpeed");
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
            UpdateAnimator();
        }
        #endregion

        #region PublicFuncionality
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            _actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _navMeshAgent.SetDestination(destination);
            _navMeshAgent.speed = _maxSpeed * Mathf.Clamp01(speedFraction);
            _navMeshAgent.isStopped = false;
        }


        #endregion

        #region PrivateFunctionality
        private void UpdateAnimator()
        {
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            
            _animator.SetFloat(_ForwardSpeedAnimId, speed);
        }

        private void Death()
        {
            _navMeshAgent.enabled = false;
            enabled = false;
        }
        #endregion

        #region Interfaces
        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }
        #endregion
    }
}
