using System;
using UnityEngine;

//Program Files (x86)\Unity\<engine version>\Editor\Data\Resources\ScriptTemplates
namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Trigger Animation Effect", menuName = "Abilities/Effects/TriigerAnimation")]
    public class TriggerAnimationEffect : EffectStrategy
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        string _animationTrigger;
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
        public override void StartEffect(AbilityData data, Action finished)
        {
            Animator animator = data.User.GetComponent<Animator>();
            animator.SetTrigger(_animationTrigger);
            finished();
        }
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        #endregion
    }
}
