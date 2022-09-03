using UnityEngine;

using RPG.Attributes;
using RPG.Control;
using RPG.Core;

namespace RPG.Combat
{
    [RequireComponent(typeof(HitPoints))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        #region Interfaces
        public bool HandleRaycast(PlayerController playerController)
        {
            if(!enabled)
                return false;
                
            if(playerController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                if (InputManager.IsPointerPressed())
                    playerController.GetComponent<Fighter>().Attack(gameObject);

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