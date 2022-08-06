using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;

using RPG.Saving;

namespace RPG.SceneManagment
{    
    public class SavingWrapper : MonoBehaviour
    {      
        #region Parameters
        [SerializeField]
        private float _fadeInTime = 1f;
        #endregion

        #region StaticVariables
        private const string DEFAULT_SAVE_FILE = "save";
        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        private IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            yield return GetComponent<SavingSystem>().LoadLastScene(DEFAULT_SAVE_FILE);

            yield return fader.FadeIn(_fadeInTime);
        }

        private void Update()
        {
            if (Keyboard.current.sKey.wasPressedThisFrame)
                Save();
            else if (Keyboard.current.lKey.wasPressedThisFrame)
                Load();
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
        #endregion
    }
}