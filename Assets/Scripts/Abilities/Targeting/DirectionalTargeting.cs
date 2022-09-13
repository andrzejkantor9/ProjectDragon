using System;
using System.Collections.Generic;

using UnityEngine;

using RPG.Control;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "DirectionalTargeting", menuName = "Abilities/Targeting/Directional")]
    public class DirectionalTargeting : TargetingStrategy
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        LayerMask _layerMask;
        [SerializeField]
        float _groundOffset = 1f;
        #endregion

        #region Cache
        //[Header("CACHE")]
        //[Space(8f)]
        #endregion

        #region States
        #endregion

        #region Events & Statics
        //[Header("EVENTS")]
        //[Space(8f)]
        const float MAX_POINTER_RAYCAST_DISTANCE = 1000f;
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods & Contructors
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces & Inheritance
        public override void StartTargeting(AbilityData data, Action finished)
        {
            RaycastHit pointerHit;
            Ray pointerRay = PlayerController.GetPointerRay();
            if (Physics.Raycast(pointerRay, out pointerHit
                , MAX_POINTER_RAYCAST_DISTANCE, _layerMask))
            {
                data.SetTargetedPoint(
                    pointerHit.point + pointerRay.direction * _groundOffset / pointerRay.direction.y);
            }

            finished();
        }
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        #endregion
    }
}
