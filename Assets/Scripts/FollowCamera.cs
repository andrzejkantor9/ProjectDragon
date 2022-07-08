using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void Update()
    {
        transform.position = _target.position;
    }
    #endregion
}
