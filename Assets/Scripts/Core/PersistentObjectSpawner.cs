using System;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        #region Cache
        [SerializeField]
        private GameObject _persistentObjectPrefab;
        #endregion

        #region States
        private static bool _hasSpawned;
        #endregion

        ///////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            if(_hasSpawned) return;

            SpawnPersistentObjects();

            _hasSpawned = true;   
        }
        #endregion

        #region PrivateMethods
        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(_persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
        #endregion
    }
}
