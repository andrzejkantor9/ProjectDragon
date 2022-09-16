using UnityEngine;

using RPG.Core;
using RPG.Control;
using RPG.SceneManagment;

namespace RPG.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        #region Config
        //[Header("CONFIG")]
        #endregion

        #region Cache
        //[Header("CACHE")]
        //[Space(8f)]
        PlayerController _playerController;
        SavingWrapper _savingWrapper;
        #endregion

        #region States
        #endregion

        #region Events & Statics
        //[Header("EVENTS")]
        //[Space(8f)]
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods & Contructors
        void Awake() 
        {
            _playerController = GameManager.PlayerGameObject.GetComponent<PlayerController>();
        }

        void OnEnable() 
        {
            Time.timeScale = 0f;
            if(_playerController)
                _playerController.enabled = false;
        }

        void OnDisable() 
        {
            Time.timeScale = 1f;
            if(_playerController)
                _playerController.enabled = true;
        }
        #endregion

        #region PublicMethods
        public void Save()
        {
            CheckAndSetSavingWrapper();
            _savingWrapper.Save();
        }

        public void SaveAndQuit()
        {
            CheckAndSetSavingWrapper();
            _savingWrapper.Save();
            Quit();
        }

        public void Quit()
        {
            CheckAndSetSavingWrapper();
            _savingWrapper.LoadMainMenu();
        }
        #endregion

        #region Interfaces & Inheritance
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        void CheckAndSetSavingWrapper()
        {
            if (!_savingWrapper)
                _savingWrapper = FindObjectOfType<SavingWrapper>();
        }
        #endregion
    }
}
