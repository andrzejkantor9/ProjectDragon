using UnityEngine;

using RPG.Core;

using GameDevTV.Inventories;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        #region Cache
        private Pickup _pickup;
        #endregion

        //////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake() => _pickup = GetComponent<Pickup>();
        #endregion

        #region Interfaces
        public bool HandleRaycast(PlayerController playerController)
        {
            if(InputManager.WasPointerPressedThisFrame())
                _pickup.PickupItem();

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
