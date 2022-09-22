using UnityEngine;

using RPG.Attributes;
using RPG.Interactions;
using RPG.Core;

namespace RPG.Combat
{
    [RequireComponent(typeof(HitPoints))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        #region EngineMethods
        //so we can enable / disable in editor
        private void Start() 
        {
        }
        #endregion

        #region Interfaces
        public bool HandleRaycast(GameObject player)
        {
            if(!enabled)
                return false;
                
            if(player.GetComponent<Fighter>().CanAttack(gameObject))
            {
                if (InputManager.IsPointerPressed())
                    player.GetComponent<Fighter>().Attack(gameObject);

                return true;                        
            }

            return false;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
        #endregion
    }
}