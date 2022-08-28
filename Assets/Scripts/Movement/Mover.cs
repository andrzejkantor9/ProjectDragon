using UnityEngine;
using UnityEngine.AI;

using RPG.Core;
using GameDevTV.SavingAssetPack;
using RPG.Attributes;

//fix nullrefs when starting and changing scene
    //make transitions from levels work properly
//fix enemies hp bars
//fix on enemy kill nullrefs

//fix upward rotation when facing enemy on elevation

//MAKE move to clicked area component
//decouple animations
//inject navmesh & make it a pure c# class
//inject action on which it should decide if click to move should apply
//make any point input work
//pass input action instead of doing it here
//remember it should work out of the box, when adding component - as little extra work as possible
//make editor functionality to find ALL things referencing script (enum as int placing in middle a new state)

//TODO make checker if all levels values are set in progression
//TODO internal vs public vs etc
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

//TODO add audio
//TODO attacked enemies auto chase

//TODO add list of buffs like in bdo and prep phase
namespace RPG.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(HitPoints))]
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        #region Parameters
        [SerializeField]
        private float _maxSpeed = 6f;
        [SerializeField]
        private float _maxNavMeshPathLength = 40f;
        #endregion

        #region Cache
        [HideInInspector]
        private NavMeshAgent _navMeshAgent;
        [HideInInspector]
        private Animator _animator;
        [HideInInspector]
        private ActionScheduler _actionScheduler;
        [HideInInspector]
        private HitPoints _health;

        private int _ForwardSpeedAnimId;
        #endregion

        ///////////////////////////////////////////////////////

        #region EngineFunctionality
        private void OnValidate() 
        {        
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _health = GetComponent<HitPoints>();
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

        #region PublicMethods
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            _actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath navMeshPath = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, navMeshPath);
            if(!hasPath || navMeshPath.status != NavMeshPathStatus.PathComplete)
                return false;
            if(GetPathLength(navMeshPath) > _maxNavMeshPathLength)
                return false;

            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _navMeshAgent.SetDestination(destination);
            _navMeshAgent.speed = _maxSpeed * Mathf.Clamp01(speedFraction);
            _navMeshAgent.isStopped = false;
        }
        #endregion

        #region PrivateMethods
        private float GetPathLength(NavMeshPath navMeshPath)
        {
            float totalDistance = 0f;
            if(navMeshPath.corners.Length < 2f)
                return totalDistance;

            for(int i =0; i < navMeshPath.corners.Length - 1; ++i)
            {
                totalDistance += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i+1]);
            }

            return totalDistance;
        }

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
            bool navMeshEnabled = _navMeshAgent.enabled;
            _navMeshAgent.enabled = true;

            _navMeshAgent.isStopped = true;
            _navMeshAgent.enabled = navMeshEnabled;
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
