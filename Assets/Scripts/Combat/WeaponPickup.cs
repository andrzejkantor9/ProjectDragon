using System.Collections;

using UnityEngine;

using RPG.Control;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        #region Parameters
        [SerializeField]
        private WeaponConfig _weapon;
        [SerializeField]
        private float _hitPointsToRestore = 0f;
        [SerializeField]
        private float _respawnTime = 5f;
        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void OnTriggerEnter(Collider other)
        {
            if(GameManager.HasPlayerTag(other.gameObject))
            {
                Pickup(other.gameObject);
            }
        }
        #endregion

        #region Interfaces
        public bool HandleRaycast(PlayerController playerController)
        {
            if(InputManager.IsPointerPressed())
            {
                // Pickup(playerController.gameObject);
                playerController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
        #endregion

        #region Coroutines
        private IEnumerator HideForSeconds(float seconds)
        {
            SetPickupActive(false);
            yield return new WaitForSeconds(seconds);
            SetPickupActive(true);
        }
        #endregion

        #region PrivateMethods
        private void SetPickupActive(bool active)
        {
            GetComponent<Collider>().enabled = active;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(active);
            }
        }

        private void Pickup(GameObject subject)
        {
            if(_weapon != null)
                subject.GetComponent<Fighter>().EquipWeapon(_weapon);
            if(_hitPointsToRestore > 0f)
                subject.GetComponent<HitPoints>().Heal(_hitPointsToRestore);
            StartCoroutine(HideForSeconds(_respawnTime));
        }
        #endregion
    }
}
