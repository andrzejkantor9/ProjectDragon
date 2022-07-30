using UnityEngine;

using UnityEngine.InputSystem;

using RPG.Movement;
using RPG.Combat;
using RPG.Core;

//TODO my own fps on screen counter

//TODO InteractWithCombat is ugly
namespace RPG.Control
{
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Fighter))]
    public class PlayerController : MonoBehaviour
    {
        #region Cache
        [HideInInspector]
        private Mover _mover;
        [HideInInspector]
        private Fighter _fighter;
        [HideInInspector]
        private Health _health;
        #endregion

        ///////////////////////////////////////////////////

        #region EngineFunctions
        private void OnValidate()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnDeath += Death;
        }

        private void OnDisable()
        {
            _health.OnDeath -= Death;
        }

        void Update()
        {
            if(InteractWithCombat())
                return;

            if(InteractWithMovement())
                return;

            Logger.Log("nothing to do.");
        }
        #endregion

        #region PrivateFunctions
        private void Death()
        {
            enabled = false;
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetPointerRay());
            
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if(target == null) continue;

                if(_fighter.CanAttack(target.gameObject))
                {
                    if (InputManager.IsPointerPressed())
                        _fighter.Attack(target.gameObject);

                    return true;                        
                }
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetPointerRay(), out hit);

            if (hasHit)
            {
                if (InputManager.IsPointerPressed())
                    _mover.StartMoveAction(hit.point, 1f);

                return true;
            }

            return false;
        }

        private static Ray GetPointerRay()
        {
            return Camera.main.ScreenPointToRay(InputManager.GetPointerPosition());
        }
        #endregion
    }
}
