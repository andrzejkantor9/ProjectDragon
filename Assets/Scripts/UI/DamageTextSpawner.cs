using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        #region Cache
        [SerializeField]
        private DamageText _damageTextPrefab;
        #endregion

        ////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_damageTextPrefab, $"_damageTextPrefab is null on: {gameObject.name}");
        }
        #endregion

        #region PublicMethods
        public void Spawn(float damageAmout)
        {
            DamageText damageText = Instantiate<DamageText>(_damageTextPrefab, transform.position, Quaternion.identity);
            damageText.SetValue(damageAmout);
        }
        #endregion
    }
}