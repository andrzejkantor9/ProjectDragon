using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField][Tooltip("if null destroys gameObject")]
        private GameObject _targetToDestroy = null;

        ////////////////////////////////////////////////////////

        #region EngineMethods
        private void Update()
        {
            if(!GetComponent<ParticleSystem>().IsAlive())
            {
                if(_targetToDestroy)
                    Destroy(_targetToDestroy);
                else
                    Destroy(gameObject);
            }
        }
        #endregion
    }
}