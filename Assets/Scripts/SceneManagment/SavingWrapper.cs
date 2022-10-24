using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using RPG.Saving;
using System;

namespace RPG.SceneManagment
{    
    public class SavingWrapper : MonoBehaviour
    {      
        #region Parameters
        [SerializeField]
        float _fadeInTime = 1f;
        [SerializeField]
        float _fadeOutTime = 1f;
        [SerializeField]
        SavingInputConfig _savingInputConfig;
        [SerializeField]
        int _newGameSceneIndex = 1;
        [SerializeField]
        int _mainMenuSceneIndex = 0;
        #endregion

        #region Statics
        const string CURRENT_SAVE_KEY = "CurrentSaveName";
        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        void Update()
        {
#if UNITY_DEVELOPMENT || UNITY_EDITOR
            if(_savingInputConfig.WasSavePressedThisFrame)
                Save();
            if(_savingInputConfig.WasLoadPressedThisFrame)
                Load();
            if(_savingInputConfig.WasDeleteSavePressedThisFrame)
                DeleteSave();
#endif
        }
        #endregion

        #region PublicFunctions
        public void ContinueGame()
        {    
            if(!PlayerPrefs.HasKey(CURRENT_SAVE_KEY)
                || !GetComponent<SavingSystem>().SaveFileExists(GetCurrentSave()))
                return;

            StartCoroutine(LoadLastScene());
            UnityEngine.Assertions.Assert.IsNotNull(_savingInputConfig, "_savingInputConfig is null");
        }

        public void NewGame(string saveFile)
        {
            if(string.IsNullOrWhiteSpace(saveFile))
                return;

            SetCurrentSave(saveFile);
            StartCoroutine(LoadScene(_newGameSceneIndex));
        }

        public void LoadMainMenu()
        {
            StartCoroutine(LoadScene(_mainMenuSceneIndex));
        }

        public void LoadGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            ContinueGame();
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(GetCurrentSave());
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(GetCurrentSave());
        }

        public void DeleteSave()
        {
            GetComponent<SavingSystem>().Delete(GetCurrentSave());
        }

        public IEnumerable<string> ListSaves() => GetComponent<SavingSystem>().ListSaves();
        #endregion

        #region PrivateMethods
        IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(_fadeOutTime);

            yield return GetComponent<SavingSystem>().LoadLastScene(GetCurrentSave());
            yield return fader.FadeIn(_fadeInTime);
        }
        
        IEnumerator LoadScene(int buildIndex)
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(_fadeOutTime);

            yield return SceneManager.LoadSceneAsync(buildIndex);
            yield return fader.FadeIn(_fadeInTime);
        }

        void SetCurrentSave(string saveFile)
        {
            PlayerPrefs.SetString(CURRENT_SAVE_KEY, saveFile);
        }

        string GetCurrentSave()
        {
            return PlayerPrefs.GetString(CURRENT_SAVE_KEY);
        }
        #endregion
    }
}