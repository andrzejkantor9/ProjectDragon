using System;
using System.Collections.Generic;

using UnityEngine;

// [CreateAssetMenu(fileName = "TargetingStrategy", menuName = "")]

namespace RPG.Abilities
{
    public abstract class FilterStrategy : ScriptableObject
    {
        #region Config
        //[Header("CONFIG")]
        #endregion

        #region Cache
        //[Header("CACHE")]
    	//[Space(8f)]
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

        #region EngineMethods
        #endregion

        #region PublicMethods
        public abstract IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter);
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
