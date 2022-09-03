using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogue
{
    public class DialogueTrigger : MonoBehaviour 
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        private string action;
        [SerializeField]
        private UnityEvent onTrigger;
        #endregion

        //////////////////////////////////////////////////////////////////

        #region PublicMethods
        public void Trigger(string actionToTrigger)
        {
            if(actionToTrigger == action)
            {
                onTrigger?.Invoke();
            }
        }
        #endregion
    }
}