using System;

using UnityEngine;

using TMPro;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        #region Cache
        [SerializeField]
        private TMP_Text _damageText;
        #endregion

        ////////////////////////////////////////////

        #region PublicMethods
        public void DestroyText() => Destroy(gameObject);

        public void SetValue(float value)
        {
            _damageText.text = String.Format("{0:0}", value);
        }
        #endregion
    }
}