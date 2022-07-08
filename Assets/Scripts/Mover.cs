using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;
using UnityEngine.InputSystem;

//MAKE move to clicked area component

//TODO use cinemachine instead of camera
public class Mover : MonoBehaviour
{
    #region Cache
    [HideInInspector]
    private NavMeshAgent _navMeshAgent;
    [HideInInspector]
    private Animator _animator;

    private int _ForwardSpeedAnimId;
    #endregion

    ///////////////////////////////////////////////////////

    #region EngineFunctionality
    private void OnValidate() 
    {        
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        _ForwardSpeedAnimId = Animator.StringToHash("ForwardSpeed");
    }

    private void Update() 
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
            MoveToCursor();

        UpdateAnimator();
    }
    #endregion

    #region PrivateFunctionality
    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);

        if(hasHit)
            _navMeshAgent.SetDestination(hit.point);
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = _navMeshAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        
        GetComponent<Animator>().SetFloat(_ForwardSpeedAnimId, speed);
    }
    #endregion
}
