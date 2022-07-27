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
    public class Mover : MonoBehaviour, IAction
    {
        #region Cache
        [HideInInspector]
        private NavMeshAgent _navMeshAgent;
        [HideInInspector]
        private Animator _animator;
        [HideInInspector]
        private ActionScheduler _actionScheduler;

        private int _ForwardSpeedAnimId;
        #endregion

        ///////////////////////////////////////////////////////

        #region EngineFunctionality
        private void OnValidate() 
        {        
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        private void Awake()
        {
            _ForwardSpeedAnimId = Animator.StringToHash("ForwardSpeed");
        }

        private void Update() 
        {
            UpdateAnimator();
        }
        #endregion

        #region PublicFuncionality
        public void StartMoveAction(Vector3 destination)
        {
            _actionScheduler.StartAction(this);
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            _navMeshAgent.SetDestination(destination);
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
        #endregion

        #region Interfaces
        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }
        #endregion
    }
}
