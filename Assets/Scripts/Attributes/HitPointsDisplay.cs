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
        private TMP_Text _text;
        #endregion

        ///////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _hitPoints = GameManager.PlayerGameObject().GetComponent<HitPoints>();   
        }

        private void Update()
        {
            _text.text = _hitPoints.HitPointsValue.value.ToString("0.") + " / " + _hitPoints.GetMaxHitPoints().ToString("0.");
        }
        #endregion
    }
}
