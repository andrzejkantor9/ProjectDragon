using System;

using UnityEngine;

using TMPro;

using RPG.Core;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        #region Cache
        [HideInInspector]
        private Experience _experience;
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
            _experience = GameManager.PlayerGameObject.GetComponent<Experience>();   
        }

        private void Update()
        {
            _text.text = _experience.GetLevel.ToString("0.");
        }
        #endregion
    }
}
