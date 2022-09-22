using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

using RPG.Core;

using RPG.Debug;

namespace RPG.SceneManagment
{
    public class Portal : MonoBehaviour
    {
        #region Parameters
        [SerializeField]
        private int _sceneToLoad = -1;
        [SerializeField]
        private DestinationIdentifier _destination;

        [Header("Fading")]
        [SerializeField]
        private float _fadeOutDuration = .5f;
        [SerializeField]
        private float _fadeInDuration = 1f;
        [SerializeField]
        private float _fadeOutDelay = .5f;
        #endregion

        #region Cache
        [field: SerializeField]
        public Transform SpawnPoint {get; private set;}
        #endregion

        #region Enums
        private enum DestinationIdentifier
        {
            A, B, C, D, E
        }
        #endregion

        #region Statics
        private static List<Coroutine> _runningCoroutines = new List<Coroutine>();
        #endregion

        ////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsNotNull(SpawnPoint, "_spawnPoint is not set");
        }

        private void OnTriggerEnter(Collider other)
        {            
            if(other.CompareTag(Enums.EnumToString<Tags>(Tags.Player)))
            {
                // foreach (Coroutine coroutine in _runningCoroutines)
                // {
                //     StopCoroutine(coroutine);
                //     _runningCoroutines.Remove(coroutine);
                // }
                StopAllCoroutines();

                Coroutine transitionSceneCoroutine = StartCoroutine(TransitionScene());
                // _runningCoroutines.Add(transitionSceneCoroutine);
            }
        }
        #endregion

        #region PrivateMethods
        private IEnumerator TransitionScene()
        {
            //yield break
            DontDestroyOnLoad(gameObject);
            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            // PlayerController playerController = GameManager.PlayerGameObject.GetComponent<PlayerController>();
            // playerController.enabled = false;
            GameManager.PlayerGameObject().SetActive(false);
            
            yield return fader.FadeOut(_fadeOutDuration);
            // _runningCoroutines.Remove(fadeOut);

            savingWrapper.Save();
            CustomLogger.Log("loading scene", LogFrequency.Rare);
            yield return LoadSceneByIndexAsync(_sceneToLoad);

            // PlayerController newPlayerController = GameManager.PlayerGameObject.GetComponent<PlayerController>();
            // newPlayerController.enabled = false;
            GameManager.PlayerGameObject().SetActive(false);

            CustomLogger.Log("scene loaded", LogFrequency.Rare);
            savingWrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            savingWrapper.Save();

            yield return new WaitForSeconds(_fadeOutDelay);
            fader.FadeIn(_fadeInDuration);
            // _runningCoroutines.Remove(fadeIn);
    
            // newPlayerController.enabled = true;
            GameManager.PlayerGameObject().SetActive(true);
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            // return;
            GameObject playerGameObject = GameManager.PlayerGameObject();

            playerGameObject.GetComponent<NavMeshAgent>().Warp(otherPortal.SpawnPoint.position);
            playerGameObject.transform.rotation = otherPortal.SpawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if(portal._destination == this._destination && portal != this) 
                    return portal;
            }

            return null;
        }
        #endregion

        #region StaticMethods
        private static void LoadSceneByIndex(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        private static AsyncOperation LoadSceneByIndexAsync(int sceneIndex)
        {
            return SceneManager.LoadSceneAsync(sceneIndex);
        }

        private static void LoadNextScene()
        {
            int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneBuildIndex = SceneManager.sceneCountInBuildSettings == currentSceneBuildIndex + 1 ? 0 : currentSceneBuildIndex + 1;

            SceneManager.LoadScene(nextSceneBuildIndex);
            CustomLogger.Log($"next scene build index: {nextSceneBuildIndex}, current scene build index: {currentSceneBuildIndex}", LogFrequency.Rare);
        }
        #endregion
    }
}