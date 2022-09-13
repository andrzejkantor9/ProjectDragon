using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using RPG.Control;
using RPG.Core;
using RPG.Debug;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "DelayedClickTargeting", menuName = "Abilities/Targeting/DelayedClick")]
    public class DelayedClickTargeting : TargetingStrategy
    {
        #region Config
        [SerializeField]
        private Texture2D _cursorTexture;
        [SerializeField]
        private Vector2 _cursorHotspot;
        [SerializeField]
        private float _areaEffectRadius = 5f;
        [SerializeField]
        private LayerMask _layerMask;
        #endregion

        #region Cache
        [Header("CACHE")]
        [Space(8f)]
        [SerializeField]
        private Transform _targetingPrefab;

        private Transform _targetingPrefabInstance = null;
        #endregion

        #region States
        #endregion

        #region Events
        //[Header("EVENTS")]
        //[Space(8f)]
        #endregion

        #region Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethodsx
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces & Inheritance
        public override void StartTargeting(AbilityData data, Action finished)
        {
            PlayerController playerController = data.User.GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(data, playerController, finished));
        }
        #endregion

        #region Events
        #endregion

        #region StaticMethods
        #endregion

        #region PrivateMethods
        private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action finished)
        {
            playerController.enabled = false;
            if(_targetingPrefabInstance == null)
            {
                _targetingPrefabInstance = Instantiate(_targetingPrefab);                
            }
            else
            {
                _targetingPrefabInstance.gameObject.SetActive(true);                
            }
            _targetingPrefabInstance.localScale = new Vector3(_areaEffectRadius*2f, 1f, _areaEffectRadius*2f);

            while(!data.Cancelled)
            {
                Cursor.SetCursor(_cursorTexture, _cursorHotspot, CursorMode.Auto);
                const float MAX_POINTER_RAY_DISTANCE = 1000f;
                RaycastHit pointerHit;

                if(Physics.Raycast(PlayerController.GetPointerRay(), out pointerHit, MAX_POINTER_RAY_DISTANCE, _layerMask))
                {
                    _targetingPrefabInstance.position = pointerHit.point;

                    if(InputManager.WasPointerPressedThisFrame())
                    {
                        //done to absorb whole mouse click
                        yield return new WaitWhile(() => InputManager.IsPointerPressed());            
                        
                        data.SetTargetedPoint(pointerHit.point);
                        data.SetTargets(GetGameObjectsInRadius(pointerHit.point));

                        break;
                    }
                }   
                yield return null;             
            }

            _targetingPrefabInstance.gameObject.SetActive(false);
            playerController.enabled = true;
            finished();
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 abilityCastPoint)
        {
            RaycastHit[] abilityHits = Physics.SphereCastAll(abilityCastPoint, _areaEffectRadius, Vector3.up, 0f);
            foreach(RaycastHit abilityHit in abilityHits)
            {
                yield return abilityHit.collider.gameObject;
            }
        }
        #endregion
    }
}
