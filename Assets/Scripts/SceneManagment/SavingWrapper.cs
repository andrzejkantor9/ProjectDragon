using System.Collections;

using UnityEngine;

using GameDevTV.SavingAssetPack;

namespace RPG.SceneManagment
{    
    public class SavingWrapper : MonoBehaviour
    {      
        #region Parameters
        [SerializeField]
        private float _fadeInTime = 1f;
        [SerializeField]
        private SavingInputConfig _savingInputConfig;
        #endregion

        #region StaticVariables
        private const string DEFAULT_SAVE_FILE = "save";
        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        
        private void Awake()
        {    
            StartCoroutine(LoadLastScene());
            UnityEngine.Assertions.Assert.IsNotNull(_savingInputConfig, "_savingInputConfig is null");
        }

        private void Update()
        {
            if(_savingInputConfig.WasSavePressedThisFrame)
                Save();
            if(_savingInputConfig.WasLoadPressedThisFrame)
                Load();
            if(_savingInputConfig.WasDeleteSavePressedThisFrame)
                DeleteSave();
        }
        #endregion

        #region PublicFunctions
        public void Load()
        {
            GetComponent<SavingSystem>().Load(DEFAULT_SAVE_FILE);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(DEFAULT_SAVE_FILE);
        }

        public void DeleteSave()
        {
            GetComponent<SavingSystem>().Delete(DEFAULT_SAVE_FILE);
        }

        #region Coroutines
        private IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(DEFAULT_SAVE_FILE);
            
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            yield return fader.FadeIn(_fadeInTime);
        }
        #endregion
        #endregion
    }
}