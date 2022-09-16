using System.Collections;

using UnityEngine;
using UnityEngine.AI;

using Cinemachine;

using RPG.Attributes;
using RPG.SceneManagment;
using System;

//Program Files (x86)\Unity\<engine version>\Editor\Data\Resources\ScriptTemplates
namespace RPG.Control
{
    public class Respawner : MonoBehaviour
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        Transform _respawnLocation;
        [SerializeField]
        float _respawnDelay = 3f;
        [SerializeField]
        float _fadeTime = .5f;
        [SerializeField]
        float _hitPointsRegenPercentage = 20f;
        [SerializeField]
        float _enemyHitPointsRegenPercentage = 20f;
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
        private void Awake() 
        {
            HitPoints hitPoints = GetComponent<HitPoints>();
            if(hitPoints && hitPoints)
                hitPoints.OnDeath += Respawn;
        }

        private void Start() 
        {
            HitPoints hitPoints = GetComponent<HitPoints>();
            if(hitPoints && hitPoints.IsDead)
            {
                Respawn();
            }
        }

        private void OnDestroy() 
        {
            HitPoints hitPoints = GetComponent<HitPoints>();
            if(hitPoints && hitPoints)
                hitPoints.OnDeath -= Respawn;
        }
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces & Inheritance
        #endregion

        #region Events & Statics
        void Respawn()
        {
            StartCoroutine(RespawnRoutine());
        }
        #endregion

        #region PrivateMethods
        private IEnumerator RespawnRoutine()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            yield return new WaitForSeconds(_respawnDelay);

            var fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(_fadeTime);

            RespawnPlayer();
            ResetEnemies();
            savingWrapper.Save();

            yield return fader.FadeIn(_fadeTime);
        }

        private void ResetEnemies()
        {
            foreach(AIController enemyController in FindObjectsOfType<AIController>())
            {
                var hitPoints = enemyController.GetComponent<HitPoints>();
                if(hitPoints && !hitPoints.IsDead)
                {
                    enemyController.Reset();
                    hitPoints.Heal(hitPoints.GetMaxHitPoints() * _enemyHitPointsRegenPercentage / 100f);
                }
            }
        }

        private void RespawnPlayer()
        {
            Vector3 positionDelta = _respawnLocation.position - transform.position;
            GetComponent<NavMeshAgent>().Warp(_respawnLocation.position);

            var hitPoints = GetComponent<HitPoints>();
            hitPoints.Heal(hitPoints.GetMaxHitPoints() * _hitPointsRegenPercentage / 100f);
            // hitPoints.Respawn();

            ICinemachineCamera activeVirtualCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;
            if(activeVirtualCamera.Follow == transform)
            {
                activeVirtualCamera.OnTargetObjectWarped(transform, positionDelta);
            }
        }
        #endregion
    }
}
