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
        [HideInInspector]
        private TMP_Text _text;
        #endregion

        ///////////////////////////////////////////////////////////////

        #region EngineMethods
        private void OnValidate()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Awake()
        {
            _manaPoints = GameManager.PlayerGameObject().GetComponent<Mana>();   
        }

        private void Update()
        {
            _text.text = _manaPoints.ManaPoints.value.ToString("0.") + " / " + _manaPoints.GetMaxMana().ToString("0.");
        }
        #endregion
    }
}
