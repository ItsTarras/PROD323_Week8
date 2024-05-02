using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Move
public class PlayerUnitControl : MonoBehaviour
{
    [SerializeField] float _targetRange = 0.5f;
    NavMeshAgent navMeshAgent;
    bool _isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    public void Select()
    {
        _isSelected = true;
    }

    public void Unselect()
    {
        _isSelected = false;
    }

    public bool IsSelected()
    {
        return _isSelected;
    }

    public void StartUpdateTargetPosition(Vector3 target)
    {
        GetComponent<PlayerUnitFSM>().Cancel();
        UpdateTargetPosition(target);
    }

    public void UpdateTargetPosition(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
        navMeshAgent.isStopped = false;
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }

    public void MoveFacingTowardsTarget(Transform target)
    {
        int damping = 2;

        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    public void Cancel()
    {
        navMeshAgent.isStopped = true;
    }

    // Update is called once per frame
    void Update()
    {
        if( Vector3.Distance(transform.position, navMeshAgent.destination) < _targetRange)
        {
            Cancel();
        }

        UpdateAnimator();
    }
}