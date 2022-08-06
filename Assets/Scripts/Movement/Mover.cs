using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using RPG.Core;
using RPG.Saving;

//MAKE move to clicked area component
//decouple animations
//inject navmesh & make it a pure c# class
//inject action on which it should decide if click to move should apply
//make any point input work
//pass input action instead of doing it here
//remember it should work out of the box, when adding component - as little extra work as possible

//TODO use addressables instead of resources
//TODO fix fading bug if switching scenes fast
//TODO use custom ienumerator with yield return
    //get one card from deck? (or queue / stack? - lazily produce value?)
    //wait for certain conditions (boss hp, player position)
    //chain quests, story state
    //book pages
//TODO should i initialize variables or not needed
//TODO debug script calling functions on object's components
//TODO make everything possible as standalone components (for any unity project)
//TODO save data in editor without serializefield for objects instantiated in runtime (hide in inspector + on validate does not work)
//TODO use cinemachine instead of camera
namespace RPG.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Health))]
    public class Mover : MonoBehaviour, IAction, ISaveable
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

        // // [System.Serializable]
        // // private struct MoverSaveData
        // // {
        // //     public SerializableVector3 position;
        // //     public SerializableVector3 rotation;
        // // }

        public object CaptureState()
        {
            // Dictionary<string, object> data = new Dictionary<string, object>();
            // data["position"] = new SerializableVector3(transform.position);
            // data["rotation"] = new SerializableVector3(transform.eulerAngles);

            // MoverSaveData data = new MoverSaveData();
            // data.position = new SerializableVector3(transform.position);
            // data.rotation = new SerializableVector3(transform.eulerAngles);

            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            // Dictionary<string, object> data = (Dictionary<string, object>)state;
            // GetComponent<NavMeshAgent>().Warp(
            //     ((SerializableVector3)data["position"]).ToVector());
            // transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();

            // MoverSaveData data = (MoverSaveData)state;
            // GetComponent<NavMeshAgent>().Warp(data.position.ToVector());
            // transform.eulerAngles = data.rotation.ToVector();

            GetComponent<NavMeshAgent>().Warp(((SerializableVector3)state).ToVector());
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        #endregion
    }
}
