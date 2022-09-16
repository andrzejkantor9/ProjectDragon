using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Utilities;

using RPG.SceneManagment;

namespace RPG.UI
{
    public class SaveLoadUI : MonoBehaviour
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        Transform _contentRoot;
        [SerializeField]
        GameObject _buttonPrefab;
        #endregion

        #region Cache
        //[Header("CACHE")]
    	//[Space(8f)]
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
        private void OnEnable() 
        {
            DestroyObjects.DestroyWithSpecifiedParameters(_contentRoot.gameObject, false, true);
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            if(!savingWrapper)
                return;

            foreach(string save in savingWrapper.ListSaves())
            {
                GameObject buttonGameObject = Instantiate(_buttonPrefab, _contentRoot);
                TMP_Text buttonText = buttonGameObject.GetComponentInChildren<TMP_Text>();
                buttonText.text = save;

                Button button = buttonGameObject.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    savingWrapper.LoadGame(save);
                });
            }
        }
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces & Inheritance
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        #endregion
    }
}
