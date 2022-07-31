using System.Collections;

using UnityEngine;

namespace RPG.SceneManagment
{
    public class Fader : MonoBehaviour
    {
        #region Cache
        private CanvasGroup _canvasGroup;
        #endregion

        //////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();            
        }
        #endregion

        #region PublicMethods
        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1f;
        }
        #endregion

        #region Coroutines
        public IEnumerator FadeOut(float time)
        {
            //it is clamped to 1
            while(_canvasGroup.alpha != 1f)
            {
                _canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            Logger.Log("start fade in");
            //it is clamped to 0
            while(_canvasGroup.alpha != 0f)
            {
                _canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
            Logger.Log("finish fade in");
        }
        #endregion
    }
}
