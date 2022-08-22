using System;

using UnityEngine;

using TMPro;

using RPG.Core;

namespace RPG.Attributes
{
    public class HitPointsDisplay : MonoBehaviour
    {
        #region Cache
        [HideInInspector]
        private HitPoints _hitPoints;
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
            _hitPoints = GameManager.PlayerGameObject.GetComponent<HitPoints>();   
        }

        private void Update()
        {
            _text.text = _hitPoints.HitPointsValue.value.ToString("0.") + " / " + _hitPoints.GetMaxHitPoints().ToString("0.");
        }
        #endregion
    }
}
