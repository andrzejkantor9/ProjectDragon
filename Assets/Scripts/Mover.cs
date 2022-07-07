using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    #region Parameters
    [SerializeField]
    private Transform _moveToObjectTransform;
    #endregion

    #region Cache
    [HideInInspector]
    private NavMeshAgent _navMeshAgent;
    #endregion

    ///////////////////////////////////////////////////////

    #region EngineFunctionality
    private void OnValidate() 
    {        
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Awake() 
    {
        UnityEngine.Assertions.Assert.IsNotNull(_moveToObjectTransform, $"{this.name} move to object has not been set");
    }

    private void Update() 
    {
        _navMeshAgent.SetDestination(_moveToObjectTransform.transform.position);
    }
    #endregion

    #region PrivateFunctionality
    #endregion    
}
