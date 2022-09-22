using System;

using UnityEngine;

using TMPro;

using RPG.Attributes;
using RPG.Core;

namespace RPG.Combat
{
    public class EnemyHitPointsDisplay : MonoBehaviour
    {
        #region Cache
        [HideInInspector]
        private Fighter _fighter;
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
            _fighter = GameManager.PlayerGameObject().GetComponent<Fighter>();
        }

        private void Update()
        {
            if(_fighter.GetTargetHitPoints() == null)
                _text.text = "N/A";
            else
            {    
                HitPoints hitPoints = _fighter.GetTargetHitPoints();            
                // _text.text = hitPoints.GetPercentage().ToString("0.") + "%";
                _text.text = hitPoints.HitPointsValue.value.ToString("0.") + " / " + hitPoints.GetMaxHitPoints().ToString("0.");
            }
        }
        #endregion
    }
}
