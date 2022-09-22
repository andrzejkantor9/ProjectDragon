using UnityEngine;

using TMPro;

using RPG.Inventories;
using RPG.Core;

namespace RPG.UI
{
    public class PurseUI : MonoBehaviour
    {
        #region Config
        // [Header("CONFIG")]
        #endregion

        #region Cache
        [Header("CACHE")]
    	//[Space(8f)]
        [SerializeField]
        private TextMeshProUGUI _balanceField;

        private Purse _playerPurse;
        #endregion

        #region States
        #endregion

        #region Events
        //[Header("EVENTS")]
    	//[Space(8f)]
        #endregion

        #region Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Start()
        {
            _playerPurse = GameManager.PlayerGameObject().GetComponent<Purse>();
            RefreshUI();

            if(_playerPurse)            
                _playerPurse.onChange += RefreshUI;
        }

        private void OnDestroy()
        {
            if(_playerPurse)            
                _playerPurse.onChange -= RefreshUI;
        }
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces
        #endregion

        #region Events
        #endregion
	
        #region StaticMethods
        #endregion

        #region PrivateMethods
        private void RefreshUI()
        {
            _balanceField.text = $"{_playerPurse.Balance:N2}";
        }
        #endregion
    }
}
