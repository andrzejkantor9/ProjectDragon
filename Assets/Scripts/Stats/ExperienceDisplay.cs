using System;

using UnityEngine;

using TMPro;

using RPG.Core;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        #region Cache
        [HideInInspector]
        private Experience _experience;
        private TMP_Text _text;
        #endregion

        ///////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _experience = GameManager.PlayerGameObject().GetComponent<Experience>();   
        }

        private void Update()
        {
            _text.text = _experience.GetPoints.ToString("0.");
        }
        #endregion
    }
}
