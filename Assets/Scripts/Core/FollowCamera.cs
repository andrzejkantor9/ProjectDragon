using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        #region Cache
        [SerializeField]
        private Transform _target;
        #endregion

        ///////////////////////////////////////////////////////////

        #region EngineFunctionality
        private void Awake() 
        {
            UnityEngine.Assertions.Assert.IsNotNull(_target, "target object is null");
        }
        private void LateUpdate()
        {
            transform.position = _target.position;
        }
        #endregion
    }
}
