using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] ArmyManager _armyManager;
    [SerializeField] float _chaseDistance = 5f;

    public Vector3 personalLastSighting;

    private NavMeshAgent _navMeshAgent;
    private Animator _anim;

    private PlayerUnitControl _currentTarget;
    private Enemy _ai;

    bool _isDead;
    protected bool _updateAnim = false;

    Health _thisHealth;

    private bool _playerInSight;
    public bool WarriorInSight { get { return _playerInSight; } }
    public bool FinishChase { get { return _playerInSight; } }
    void Start()
    {
        _ai = GetComponent<Enemy>();
        _armyManager = GameObject.Find("Army Manager").GetComponent<ArmyManager>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _thisHealth = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_thisHealth.isDead)
        {
            _isDead = true;
            _ai.Cancel();
            Cancel();
            return;
        }

        if (!_isDead)
        {
            if (_updateAnim)
            {
                UpdateAnimator();
            }

            if (WarriorWithinVisionRange())
            {
                _playerInSight = true;
                _ai.Attack(_currentTarget.transform);
            }
            else
            {
                _playerInSight = false;
            }
        }
    }

    private bool WarriorWithinVisionRange()
    {
        return ClosestDistanceToWarrior() < _chaseDistance;
    }
    private float ClosestDistanceToWarrior()
    {
        float closestDistanceToWarrior = Mathf.Infinity;
        float currentWarrior = Mathf.Infinity;

        foreach (PlayerUnitControl warrior in _armyManager.GetArmyList())
        {
            currentWarrior = Vector3.Distance(warrior.transform.position, transform.position);

            if (currentWarrior < closestDistanceToWarrior)
            {
                if(currentWarrior < _chaseDistance)
                {
                    closestDistanceToWarrior = currentWarrior;
                    _currentTarget = warrior;
                    personalLastSighting = _currentTarget.transform.position;
                }
            }
        }
        return closestDistanceToWarrior;
    }

    public void StartUpdateTargetPosition(Vector3 target)
    {
        _ai.Cancel();
        UpdateTargetPosition(target);
    }

    public void UpdateTargetPosition(Vector3 target)
    {
        _anim.SetTrigger("stopAttack");
        _anim.SetBool("move", true);

        _navMeshAgent.SetDestination(target);
        _navMeshAgent.isStopped = false;
        _updateAnim = true;
    }
    
    private void UpdateAnimator()
    {
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }

    public void RotateFacingTowardsTarget(Transform target)
    {
        int damping = 2;

        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    public void Cancel()
    {
        _navMeshAgent.isStopped = true;
        _anim.ResetTrigger("stopAttack");
        _anim.SetBool("move", false);
    }
}
