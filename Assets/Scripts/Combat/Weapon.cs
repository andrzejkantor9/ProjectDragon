using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        #region Events
        [SerializeField] 
        private UnityEvent OnHit;
        #endregion

        //////////////////////////////

        #region PublicMethods
        public void Hit()
        {
            OnHit?.Invoke();
        }
        #endregion
    }
}
