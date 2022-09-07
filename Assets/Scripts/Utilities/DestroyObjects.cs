using UnityEngine;

/// <summary>
/// Use the item at the given slot. If the item is consumable one
/// instance will be destroyed until the item is removed completely.
/// </summary>
/// <param name="user">The character that wants to use this action.</param>
/// <returns>False if the action could not be executed.</returns>
namespace Utilities
{
    /// <summary>
    /// Provides simple interface to destroy specified objects with specified parameters that can be attached
    /// in inspector.
    /// </summary>
    public class DestroyObjects : MonoBehaviour 
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        private bool _destroyAtAwake = true;
        [SerializeField]
        private bool _destroyAtStart;
        [SerializeField]
        private bool _destroySelf = true;
        [SerializeField]
        private bool _destroyChildren;
        [SerializeField]
        private GameObject[] _extraObjectsToDestroy;
        [SerializeField]
        private float _destroyDelay = 0f;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake() 
        {
            if(_destroyAtAwake)
            {
                DestroyWithSpecifiedParameters();
            }
        }

        private void Start() 
        {
            if(_destroyAtStart)
            {
                DestroyWithSpecifiedParameters();
            }
        }
        #endregion

        #region PublicMethods
        public void DestroyWithSpecifiedParameters()
        {
            if(_extraObjectsToDestroy.Length > 0)
            {
                foreach(GameObject objectToDestroy in _extraObjectsToDestroy)
                {
                    Destroy(objectToDestroy, _destroyDelay);
                }
            }

            if(_destroySelf)
            {
                Destroy(this.gameObject, _destroyDelay);
            }
            else if(_destroyChildren)
            {
                foreach(Transform child in GetComponentsInChildren<Transform>())
                {
                    if(!Object.ReferenceEquals(child.gameObject, this.gameObject))
                    {
                        Destroy(child.gameObject, _destroyDelay);
                    }
                }
            }
        }
        #endregion
    }
}