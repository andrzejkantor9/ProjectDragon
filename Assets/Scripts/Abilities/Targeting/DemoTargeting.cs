using System;
using System.Collections.Generic;

using UnityEngine;

using RPG.Debug;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Abilities/Targeting/Demo")]
    public class DemoTargeting : TargetingStrategy
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

        #region EngineMethodsx
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces & Inheritance
        public override void StartTargeting(AbilityData data, Action finished)
        {
            CustomLogger.Log("demo targeting started", LogFrequency.Regular);
            finished();
        }
        #endregion

        #region Events
        #endregion

        #region StaticMethods
        #endregion

        #region PrivateMethods
        #endregion
    }
}
