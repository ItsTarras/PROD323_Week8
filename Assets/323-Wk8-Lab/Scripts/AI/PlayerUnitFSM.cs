using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerUnitFSM : MonoBehaviour
{
    [SerializeField] float _weaponRange = 2f;
    [SerializeField] float _maxChaseDistance = 7f;
    [SerializeField] GameObject _fireParticles;
    [SerializeField] GameObject _bloodParticles;

    Vector3 _posEnemy;
    Health _target;
    Transform _currentTarget;

    bool _isInRange = false;
    PlayerUnitControl _warrior;
    [SerializeField] ArmyManager _armyManager;

    Animator _anim;

    [SerializeField] float _timeBetweenAttacks = 5f;

    float _timeSinceLastAttack = 0;

    [SerializeField] int damage = 5;
    [SerializeField] int _fireDamage = 1;
    bool _isAttacking = false;
    Enemy _attacker;
    bool _isDead;
    RaycastHit[] hits;

    Health _thisHealth;
    int enemylayerMask = 1 << 9;
    bool _justGotCancelled = false;
    bool _gotCancelled = false;

    // Start is called before the first frame update
    void Start()
    {
        _armyManager = GameObject.Find("Army Manager").GetComponent<ArmyManager>();
        _thisHealth = GetComponent<Health>();
        _warrior = GetComponent<PlayerUnitControl>();
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gotCancelled) { return; }

        if(_isDead == true) { return; }

        if (_thisHealth.isDead)
        {
            _isDead = true;
            _thisHealth.tag = "Untagged";
            _anim.SetTrigger("stopAttack");
            _target = null;
            _warrior.Cancel();

            Cancel();
            return;
        }

        if (_justGotCancelled)
        {
            StartCoroutine(JustCancelled());
        }

        if (!_isDead)
        {
            if (_target == null)
            {
                return;
            }

            if (_target.isDead)
            {
                CleanUpDeadTarget();
                return;
            }

            _timeSinceLastAttack += Time.deltaTime;

            _isInRange = Vector3.Distance(transform.position, _target.transform.position) < _weaponRange;

            if (_fireParticles != null && _isInRange)
            {
                _posEnemy = _target.transform.position;
                _posEnemy.y = 0.7f;
                _fireParticles.transform.position = _posEnemy;
            }

            if (!_isInRange)
            {
                _warrior.UpdateTargetPosition(_target.transform.position);
            }
            else
            {
                _warrior.Cancel();
                AttackBehaviour();
            }
        }
    }

    IEnumerator JustCancelled()
    {
        _justGotCancelled = false;
        _gotCancelled = true;
        yield return new WaitForSeconds(2f);
        _gotCancelled = false;
    }

    void CleanUpDeadTarget()
    {
        _target.gameObject.tag = "Untagged";
        _armyManager.UpdateEnemyList();
        _anim.SetTrigger("stopAttack");
        _target = null;
        Cancel();
    }

    private void FixedUpdate()
    {
        EnemyNear();    
    }

    public void EnemyNear()
    {
        if(_target != null) { return; }
        hits = Physics.SphereCastAll(this.transform.position, _maxChaseDistance, this.transform.forward, 4f, enemylayerMask);
        
        if(hits.Length == 0)
        {
            return;
        }
        else
        {
            Attack(ClosestDistanceToWarrior());
        }
        //if enemy is near then Attack;
        return;
    }

    private Transform ClosestDistanceToWarrior()
    {
        float closestDistanceToEnemy = Mathf.Infinity;
        float currentDistanceToEnemy = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            currentDistanceToEnemy = Vector3.Distance(hit.transform.position, transform.position);

            if (currentDistanceToEnemy < closestDistanceToEnemy)
            {
                    closestDistanceToEnemy = currentDistanceToEnemy;
                    _currentTarget = hit.transform;
                    //personalLastSighting = _currentTarget.transform;
            }
        }
        Debug.LogWarning(_currentTarget);
        return _currentTarget;
    }

    private void AttackBehaviour()
    {
        _warrior.MoveFacingTowardsTarget(_target.transform);

        if (_timeSinceLastAttack >= _timeBetweenAttacks)
        {
            //Debug.Log("Attack. Time since last attack: " + _timeSinceLastAttack);
            _anim.ResetTrigger("stopAttack");
            _anim.SetTrigger("attacking");
            _isAttacking = true;
            _timeSinceLastAttack = 0f;

        }
    }

    public void Cancel()
    {
        if (_isAttacking)
        {
            _anim.ResetTrigger("attacking");
            _anim.SetTrigger("stopAttack");
            _isAttacking = false;
        }
        _justGotCancelled = true;
        _target = null;
    }

    public void Attack(Transform enemy)
    {
        if (enemy != null)
        {
            _target = enemy.GetComponent<Health>();
        }
    }

    //Animation Event
    void Shoot()
    {
        if (_target != null)
        {
            _isInRange = Vector3.Distance(transform.position, _target.transform.position) < _weaponRange;
            if (_isInRange)
            {
                _fireParticles.GetComponent<ParticleSystem>().Play();
                _target.TakeDamage(damage);
            }
        }
    }

    void Hit()
    {
        if (_target != null)
        {
            _isInRange = Vector3.Distance(transform.position, _target.transform.position) < _weaponRange;
            if (_isInRange)
            {
                _target.TakeDamage(damage);

                _posEnemy = _target.transform.position;
                _posEnemy.y = 0.6f;
                _bloodParticles.transform.position = _posEnemy;

                _bloodParticles.GetComponent<ParticleSystem>().Play();
            }
        }
    }
}