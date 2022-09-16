using UnityEngine;

using TMPro;

using GameDevTV.Utils;

using RPG.SceneManagment;

//Program Files (x86)\Unity\<engine version>\Editor\Data\Resources\ScriptTemplates
namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        TMP_InputField _newGameNameInputField;
        #endregion

        #region Cache
        //[Header("CACHE")]
    	//[Space(8f)]
        LazyValue<SavingWrapper> _savingWrapper;
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
        private void Awake() 
        {
            _savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }
        #endregion

        #region PublicMethods
        public void ContinueGame()
        {
            _savingWrapper.value.ContinueGame();
        }

        public void NewGame()
        {
            _savingWrapper.value.NewGame(_newGameNameInputField.text);
        }        

        public void QuitGame()
        {
#if !UNITY_EDITOR
            Application.Quit();
#else
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        #endregion

        #region Interfaces & Inheritance
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        SavingWrapper GetSavingWrapper()
            => FindObjectOfType<SavingWrapper>();
        #endregion
    }
}
