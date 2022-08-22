using UnityEngine;

using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/MakeNewWeapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        #region Parameters
        [field: SerializeField]
        public float WeaponRange {get; private set;} = 2f;
        [field: SerializeField]
        public float TimeBetweenAttacks {get; private set;} = 2f;
        [field: SerializeField]
        public float WeaponDamage {get; private set;} = 5f;
        [field: SerializeField]
        public float PercentageModifier {get; private set;} = 0f;
        [SerializeField]
        private bool _isRightHanded = true;
        [SerializeField]
        private Projectile projectile;
        #endregion

        #region Cache
        [SerializeField]
        private AnimatorOverrideController _weaponAnimatorOverride;
        [SerializeField]
        private Weapon equippedPrefab;

        public const string WEAPON_NAME = "Weapon";
        #endregion

        #region States
        public bool HasProjectile => projectile != null;
        #endregion

        //////////////////////////////////////////////////////////////////

        #region PublicMethods
        public void LaunchProjectile(Transform rightHand, Transform leftHand, HitPoints targetHealth, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetWeaponPosition(rightHand, leftHand).position, Quaternion.identity);
            // projectileInstance.SetTarget(targetHealth, instigator, WeaponDamage);
            projectileInstance.SetTarget(targetHealth, instigator, calculatedDamage);
        }

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;

            if(equippedPrefab != null)
            {
                weapon = Instantiate(equippedPrefab, GetWeaponPosition(rightHand, leftHand));
                weapon.gameObject.name = WEAPON_NAME;
            }
            if(_weaponAnimatorOverride != null)
                animator.runtimeAnimatorController = _weaponAnimatorOverride;
            else
            {
                var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                if(overrideController)
                    animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }
        #endregion

        #region PrivateMethods
        private Transform GetWeaponPosition(Transform rightHand, Transform leftHand) => _isRightHanded ? rightHand : leftHand;

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WEAPON_NAME);
            if(!oldWeapon)
            {
                oldWeapon = leftHand.Find(WEAPON_NAME);
                if(!oldWeapon) return;
            }

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }
        #endregion
    }
}