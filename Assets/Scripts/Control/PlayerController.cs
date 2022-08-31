using System;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;

namespace RPG.Control
{
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Fighter))]
    public class PlayerController : MonoBehaviour
    {
        #region Parameters
        [Header("Parameters")]
        [SerializeField]
        private float _navMeshProjectionDistance = 1f;
        [SerializeField]
        private float _raycastRadius = 1f;
        #endregion

        #region Cache
        [Space(8)][Header("Cache")]
        [HideInInspector]
        private Mover _mover;
        [HideInInspector]
        private Fighter _fighter;
        [HideInInspector]
        private HitPoints _hitPoints;

        [SerializeField]
        private CursorMapping[] _cursorMappings;
        #endregion

        #region States
        private bool _isDraggingUI = false;
        #endregion

        #region Data
        [Serializable]
        struct CursorMapping
        {
            [field: SerializeField]
            internal CursorType Type {get; private set;}
            
            [field: SerializeField]
            internal Texture2D Texture {get; private set;}
            [field: SerializeField]
            internal Vector2 Hotspot {get; private set;}
        }
        #endregion

        ///////////////////////////////////////////////////

        #region EngineMethods
        private void OnValidate()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _hitPoints = GetComponent<HitPoints>();
        }

        private void Start()
        {
            AudioListener.volume = .25f;
        }

        private void OnEnable()
        {
            _hitPoints.OnDeath += Death;
        }

        private void OnDisable()
        {
            _hitPoints.OnDeath -= Death;
        }

        void Update()
        {
            if(InteractWithUI())
                return;
            if(_hitPoints.IsDead)
            {
                SetCursor(CursorType.None);
                return;
            }

            if(InteractWithComponent())
                return;
            if(InteractWithMovement())
                return;

            SetCursor(CursorType.None);
        }
        #endregion

        #region PrivateMethods
        private void Death()
        {
            enabled = false;
        }

        private bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit)
            {
                if(!GetComponent<Mover>().CanMoveTo(target))
                    return false;

                if (InputManager.IsPointerPressed())
                    _mover.StartMoveAction(target, 1f);

                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            RaycastHit raycastHit;
            bool hasHit = Physics.Raycast(GetPointerRay(), out raycastHit);
            target = new Vector3();

            if(!hasHit)
                return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh 
                = NavMesh.SamplePosition(raycastHit.point, out navMeshHit, _navMeshProjectionDistance, NavMesh.AllAreas);
            if(!hasCastToNavMesh)
                return false;

            target = navMeshHit.position;
            
            return true;
        }

        private bool InteractWithUI()
        {
            if(!InputManager.IsPointerPressed())
                _isDraggingUI = false;

            bool isOverUI = EventSystem.current.IsPointerOverGameObject();
            if(isOverUI)
            {
                if(InputManager.IsPointerPressed())
                    _isDraggingUI = true;
                SetCursor(CursorType.UI);
            }

            return isOverUI || _isDraggingUI;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if(raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetPointerRay(), _raycastRadius);
            float[] distances = new float[hits.Length];

            for(int i=0; i < hits.Length; ++i)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);

            return hits;
        }

        private static Ray GetPointerRay()
        {
            return Camera.main.ScreenPointToRay(InputManager.GetPointerPosition());
        }

        private void SetCursor(CursorType cursorType)
        {
            CursorMapping cursorMapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(cursorMapping.Texture, cursorMapping.Hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach (CursorMapping cursorMapping in _cursorMappings)
            {
                if(cursorMapping.Type == cursorType)
                    return cursorMapping;
            }

            return _cursorMappings[0];
        }
        #endregion
    }
}
