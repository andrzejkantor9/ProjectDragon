using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour 
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        private bool _activateOnStart = false;
        #endregion

        #region Cache
        [Space(8f)][Header("CACHE")]
        [SerializeField]
        private Fighter[] _fighters;
        #endregion

        ///////////////////////////

        #region EngineMethods
        private void Start()
        {
            Activate(_activateOnStart);
        }
        #endregion

        #region PublicMethods
        public void Activate(bool shouldActivate)
        {
            foreach(Fighter fighter in _fighters)
            {
                CombatTarget target = fighter.GetComponent<CombatTarget>();
                if(target)
                {
                    target.enabled = shouldActivate;
                }
                fighter.enabled = shouldActivate;
            }
        }
        #endregion
    }
}