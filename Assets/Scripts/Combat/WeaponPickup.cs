using System.Collections;

using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        #region Parameters
        [SerializeField]
        private Weapon _weapon;
        [SerializeField]
        private float _respawnTime = 5f;
        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(Enums.EnumToString<Tags>(Tags.Player)))
            {
                other.GetComponent<Fighter>().EquipWeapon(_weapon);
                // Destroy(gameObject);
                StartCoroutine(HideForSeconds(_respawnTime));
            }
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
        #endregion
    }
}
