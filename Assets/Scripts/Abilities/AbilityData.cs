using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using RPG.Core;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        #region Config
        //[Header("CONFIG")]
        #endregion

        #region Cache
        //[Header("CACHE")]
        //[Space(8f)]
        #endregion

        #region States
        public GameObject User {get; private set;}
        public IEnumerable<GameObject> Targets {get; private set;}
        public Vector3 TargetedPoint {get; private set;}
        public bool Cancelled {get; private set;}
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

        #region EngineMethods
        public AbilityData(GameObject user)
        {
            User = user;
        }   
        #endregion

        #region PublicMethods
        public void SetTargets(IEnumerable<GameObject> targets) => Targets = targets;
        public void SetTargetedPoint(Vector3 targetedPoint) => TargetedPoint = targetedPoint;

        public void StartCoroutine(IEnumerator coroutine)
        {
            User.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        public void Cancel()
        {
            Cancelled = true;
        }
        #endregion

        #region Interfaces
        #endregion

        #region Events
        #endregion

        #region StaticMethods
        #endregion

        #region PrivateMethods
        #endregion
    }
}
