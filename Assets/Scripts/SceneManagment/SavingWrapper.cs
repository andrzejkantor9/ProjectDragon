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
        private float _fadeInTime = .2f;
        #endregion

        #region Cache
        const string DEFAULT_SAVE_FILE = "save";
        #endregion

        /////////////////////////////////////////////

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
            if(Keyboard.current.lKey.wasPressedThisFrame)
                Load();
            else if(Keyboard.current.sKey.wasPressedThisFrame)
                Save();
        }
        #endregion

        #region PublicMethods
        public void Load()
        {
            GetComponent<SavingSystem>().LoadLastScene(DEFAULT_SAVE_FILE);
            GetComponent<SavingSystem>().Load(DEFAULT_SAVE_FILE);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(DEFAULT_SAVE_FILE);
        }
        #endregion 
    }
}
