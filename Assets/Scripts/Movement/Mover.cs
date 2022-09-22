using UnityEngine;
using UnityEngine.AI;

using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

//bug: continue only works within 1 session?
//bug: after death / few deaths player moves by himself and camera shakes a lot
    //may be issue with navmeshagent
//bug: triggered dialogues are not loaded
//bug: continue game without a save should not do anything
//bug: make prod build and fix all bugs
//change: place portals in gameobject in hierarchy and move them properly to dont destroy
    //unpack to root utility
//add: utils - assert all given objects
//bug: fireball starts auto attacking after killing last in range enemy (check bow)
//add: destroyobjects
    //add GetPathInHierachy static helper somewhere
    //*documentation comments
//bug: dialogue skipping all non-last-pre-player-choice dialogue
//bug: respawning on 2nd level
//bug: player stats not saving between scenes

//create utils package from customlogger, enums, lazy value, log worls position, fps counter
    //document them with comments
    //shooter project

//jak widze klase to sprawdzam
    //czy klasa odpowiada tylko za logikę / kontrolę
    //czy nie ma hardcodowanych konkrecji
    //---------------------------
    //czy regiony sa dobrze nazwane, ulozone, w dobrej ilosci i gettery na gorze, namespacey w dobrej kolejnosci
    //properties vs expression body vs serialize fields vs getter
    //make all GetComponents asserted / requireComponent
    //string builder vs string concatetions
    //wrzuc assembly definitions
    //check for reasons for any null coalescing
    //any unnecesarry object creations / updates / foreach's?

//object pool & caching
//solid
//inne wzorce
//prep for translations
//script template
    //scriptable object template
    //adjust lenght of slashes / max width of code
//child object with no config mono behaviours?
//custom inspector with searching with names
//find ALL references to script / object in editor
//component to turn everything to caps or non-caps

////////////////////////////////////////////

//refactor: overlook everything
    //asserts & requireComponents
    //updates & foreaches
    //single responsibility
    //shop.cs, fighter.cs - single responsibility
    //getComponents
    //resources structure SO/<type>/resources
    //dialogue conditions as non-stringa
//bug: enemies hp bars not visible
//bug: floating characters
//bug: cannot add items in shop
//bug: on enemy / player kill nullrefs
    //archers
//bug: save creates "save.sav" file?
//bug: guards dont patrol anymore after getting in agro range when in peaceful mode
//bug: guards can be damaged with abilities in peaceful mode
//bug: npcs can be damaged with abilities
//bug: if player response is last dont display it as ai's
//change: start move action to npc dialogue instead of insta trigger
//change: dropping items should not triiger ontriggerenter pickup
//change: make abilities do "auto attacks" like attacks - unify "damage action" code
//change: tune encounter values
//change: swap equipment slots so they make sense
//add: make esc work on all menus
//add: basic audio to most places
//add: make current hud non-debug
//bug: if item is in action slot and stackable and then added it is added to inventory instead of action bar
//add: interesting blog features
//learn: if [hide in inspector] + onvalidate works in prod build
    //save data in editor without serializefield for objects instantiated in runtime (hide in inspector + on validate does not work)

//refactor:
    //base stats containing level up particle and should use modifiers?
//learn: test performance of auto layout (current version) vs pure hand layoutui
//learn: dialogueNode setter or field?
//bug: upward rotation when facing enemy on elevation
//bug: cinematic should pauses all actions
//refactor: TODO fighter on target death event -=
//refactor: assert / require GetComponents
//change: do not allow saves with the same names
//add: self scrolling text ui horizontaly
//learn: can shop.cs be split?
//change: default shop configs SOs
//add: grey out quantity shop buttons approprietely
//refactor: ui relying on being set active pre awake (in editor)
//refactor: make component that can group components in inspector
//change: make all ui into click icons in the corner (like quest ui)
//add: controller and mobile input devices
//bug: duplicating dialogue does not change its id

//learn :local methods usage overall (not this project)
//change: move quest & dialogue setup from string bindings
//add: translation setup to project, translation asset pack
//add: spoken dialogues
//add: save deletion
//dialogue
    //saving state?
    //dont trigger dialogue from any distance
    //each dialogue only once
    //scriptable objects instead of strings or enum?
    //non-strings bindings
        //https://community.gamedev.tv/t/for-those-interested-in-making-enum-predicates/171656
        //https://community.gamedev.tv/t/another-approach-to-conditions/207975/2
//dialogue editor
    //zoom in out
    //all fields in editor
    //drag node to scroll
    //some area that appears as node isnt draggable
    //flexible scroll width & height

//////////////////////////////////////////

//make any point input work
//pass input action instead of doing it here
    //what about component independency / plug & play

//move to clicked area component
//decouple animations
//inject navmesh & make it a pure c# class
//inject action on which it should decide if click to move should apply
//remember it should work out of the box, when adding component - as little extra work as possible
//*compass / map
//non-manual travel options
//day night

//split ui to multiple canvas
//draggable in game ui
//ui scale options
    //ui move option
//ui clicked on is on top
//use addressables instead of resources
//debug script calling functions on object's components
//everything possible as standalone components (for any unity project)
//add list of buffs like in bdo and prep phase
//possible use of custom ienumerator with yield return
    //get one card from deck? (or queue / stack? - lazily produce value?)
    //wait for certain conditions (boss hp, player position)
    //chain quests, story state
    //book pages

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
            UpdateAnimator();
        }
        #endregion

        #region PublicMethods
        public bool IsStopped()
        {
            return Mathf.Approximately(Vector3.Distance(_navMeshAgent.velocity, Vector3.zero), 0f) || _navMeshAgent.isStopped;
        } 

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
            SetDead(true);
        }

        private void Respawn()
        {
            SetDead(false);
        }

        private void SetDead(bool isDead)
        {
            _navMeshAgent.enabled = !isDead;
            enabled = !isDead;
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
