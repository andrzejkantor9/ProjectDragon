using System;

using UnityEngine;

using TMPro;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
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
            _experience = GameObject.FindWithTag(Enums.EnumToString<Tags>(Tags.Player)).GetComponent<Experience>();   
        }

        private void Update()
        {
            _text.text = _experience.GetPoints.ToString("0.");
        }
        #endregion
    }
}
