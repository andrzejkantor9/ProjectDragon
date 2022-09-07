using UnityEngine;

using GameDevTV.Inventories;

using RPG.Movement;
using RPG.Core;
using RPG.Interactions;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class RunOverPickup : MonoBehaviour, IRaycastable
    {
        #region Cache
        [HideInInspector]
        private Pickup _pickup;
        #endregion

        //////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake() => _pickup = GetComponent<Pickup>();

        private void OnTriggerEnter(Collider other)
        {
            if(GameManager.HasPlayerTag(other.gameObject))
                _pickup.PickupItem();
        }
        #endregion

        #region PublicMethods
        public bool HandleRaycast(GameObject playerController)
        {
            if(InputManager.IsPointerPressed())
            {
                playerController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            if(_pickup.CanBePickedUp())
                return CursorType.Pickup;
            else
                return CursorType.FullPickup;
        }
        #endregion
    }
}
