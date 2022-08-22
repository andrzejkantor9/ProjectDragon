using UnityEngine;
using UnityEngine.Playables;

using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        #region Cache
        private GameObject _playerGameObject;
        #endregion

        ///////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            _playerGameObject = GameManager.PlayerGameObject;        
        }

        private void OnEnable()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        private void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }
        #endregion

        #region PrivateMethods
        private void EnableControl(PlayableDirector playableDirector)
        {
            Logger.Log("enable control by cinematic", LogFrequency.Rare);

            _playerGameObject.GetComponent<PlayerController>().enabled = true;
        }

        private void DisableControl(PlayableDirector playableDirector)
        {
            Logger.Log("disable control by cinematic", LogFrequency.Rare);

            _playerGameObject.GetComponent<ActionScheduler>().CancelCurrentAction();
            _playerGameObject.GetComponent<PlayerController>().enabled = false;
        }
        #endregion
    }
}
