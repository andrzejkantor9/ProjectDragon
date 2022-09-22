using UnityEngine;
using UnityEngine.UI;

using TMPro;

using RPG.Stats;
using RPG.Core;

namespace RPG.UI
{
    public class TraitUI : MonoBehaviour
    {
        #region Config
        //[Header("CONFIG")]
        #endregion

        #region Cache
        [Header("CACHE")]
    	//[Space(8f)]
        [SerializeField]
        TextMeshProUGUI _unassignedPointsText;
        [SerializeField]
        Button _finalizeButton;

        TraitStore _playerTraitStore;
        BaseStats _playerBaseStats;
        #endregion

        #region States
        #endregion

        #region Events & Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods & Contructors
        private void Start() 
        {
            _playerTraitStore = GameManager.PlayerGameObject().GetComponent<TraitStore>();
            _playerBaseStats = GameManager.PlayerGameObject().GetComponent<BaseStats>();

            _finalizeButton.onClick.AddListener(_playerTraitStore.FinalizeAssigment);
            _playerTraitStore.onUpdateUI += UpdateUI;
            _playerBaseStats.onLevelUp += UpdateUI;

            UpdateUI();
        }

        private void OnDestroy() 
        {
            if(_playerTraitStore)
                _playerTraitStore.onUpdateUI -= UpdateUI;
            if(_playerBaseStats)
                _playerBaseStats.onLevelUp -= UpdateUI;
        }
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces & Inheritance
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        private void UpdateUI()
        {
            _unassignedPointsText.text = _playerTraitStore.GetUnassignedPoints().ToString();
        }
        #endregion
    }
}
