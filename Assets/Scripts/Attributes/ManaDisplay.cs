using System;

using UnityEngine;

using TMPro;

using RPG.Core;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {
        #region Cache
        [HideInInspector]
        private Mana _manaPoints;
        private TMP_Text _text;
        #endregion

        ///////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _manaPoints = GameManager.PlayerGameObject().GetComponent<Mana>();   
        }

        private void Update()
        {
            _text.text = _manaPoints.ManaPoints.value.ToString("0.") + " / " + _manaPoints.GetMaxMana().ToString("0.");
        }
        #endregion
    }
}
