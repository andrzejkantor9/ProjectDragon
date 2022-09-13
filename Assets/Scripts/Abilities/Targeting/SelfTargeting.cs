using System;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "SelfTargeting", menuName = "Abilities/Targeting/Self")]
    public class SelfTargeting : TargetingStrategy
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

        #region Events & Statics
        //[Header("EVENTS")]
        //[Space(8f)]
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
            data.SetTargets(new GameObject[]{data.User});
            data.SetTargetedPoint(data.User.transform.position);
            finished();
        }
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        #endregion
    }
}
