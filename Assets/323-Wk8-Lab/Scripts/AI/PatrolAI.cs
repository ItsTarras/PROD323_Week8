using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAI : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] Transform[] _patrolPoints;
    [SerializeField] private int _currentPatrolPointIndex;
    private int _nextPatrolPointIndex;

    [SerializeField] float _waitTime;
    [SerializeField] float _maxWaitTime = 1f;

    Transform _nextPatrolPoint;
    
    Health _thisHealth;
    AIController _ai;
    bool _isDead = false;
    

    // Start is called before the first frame update
    void Start()
    {
        _ai = GetComponent<AIController>();
        _currentPatrolPointIndex = Random.Range(0, _patrolPoints.Length);
        _nextPatrolPointIndex = _currentPatrolPointIndex;
        _nextPatrolPoint = _patrolPoints[_currentPatrolPointIndex];
        _thisHealth = GetComponent<Health>();
        _waitTime = _maxWaitTime;
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
            if (!_ai.WarriorInSight)
            {

                if (Vector3.Distance(_nextPatrolPoint.position, transform.position) <= 0.2f)
                {
                    if (_waitTime <= 0)
                    {
                        FindNextNode();
                        _ai.RotateFacingTowardsTarget(_nextPatrolPoint);
                        _waitTime = _maxWaitTime;
                    }
                    else
                    {
                        _waitTime -= Time.deltaTime;
                    }
                }
                else
                {
                    MoveToPoint(_nextPatrolPoint);
                }
            }
            else
            {
                Cancel();
            }
        }
    }

    void FindNextNode()
    {
        //Debug.Log("current index" + _nextPatrolPointIndex);

        //go this way
        _currentPatrolPointIndex++;

        _nextPatrolPoint = _patrolPoints[_currentPatrolPointIndex % _patrolPoints.Length];
        //print(_currentPatrolPointIndex);
    }

    void MoveToPoint(Transform pos)
    {
        _ai.StartUpdateTargetPosition(pos.position);
    }

    public void Cancel()
    {
        _nextPatrolPointIndex = 0;
        _nextPatrolPoint = _patrolPoints[_nextPatrolPointIndex];
        _waitTime = _maxWaitTime;
    }
}
