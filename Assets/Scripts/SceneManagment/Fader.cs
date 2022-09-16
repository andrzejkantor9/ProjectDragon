using System;
using System.Collections;

using UnityEngine;

namespace RPG.SceneManagment
{
    public class Fader : MonoBehaviour
    {
        #region Cache
        private CanvasGroup _canvasGroup;
        #endregion

        #region States
        private Coroutine _currentActiveFade;
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
        public Coroutine FadeOut(float time)
        {
             return Fade(1f, time);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0f, time);
        }

        public Coroutine Fade(float target, float time)
        {
            if(_currentActiveFade != null)
                StopCoroutine(_currentActiveFade);

            _currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return _currentActiveFade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while(!Mathf.Approximately(_canvasGroup.alpha, target))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.unscaledDeltaTime / time);
                yield return null;
            }
        }

        internal object FadeOut(object fadeOutTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
