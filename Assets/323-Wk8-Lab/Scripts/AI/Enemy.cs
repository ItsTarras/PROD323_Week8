using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attack
public class Enemy : MonoBehaviour
{
    [SerializeField] float _weaponRange = 1.5f;
    [SerializeField] Health _target;
    [SerializeField] GameObject _bloodParticles;

    bool _isInRange = false;
    AIController _ai;

    Animator _anim;
    Vector3 _posWarrior;

    [SerializeField] float _timeBetweenAttacks = 5f;

    float _timeSinceLastAttack = 0;

    [SerializeField] int damage = 5;
    bool _isAttacking = false;
    Health _thisHealth;

    bool _isDead = false;

    public bool isAttacking { get { return _isAttacking; } }

    // Start is called before the first frame update
    void Start()
    {

        _ai = GetComponent<AIController>();
        _thisHealth = GetComponent<Health>();
        if(_thisHealth == null)
        {
            Debug.Log("NOT FOUND");
        }
        _anim = GetComponent<Animator>();

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
            _timeSinceLastAttack += Time.deltaTime;

            if (_target == null)
            {
                return;
            }

            if (_target.isDead)
            {
                _anim.SetTrigger("stopAttack");
                _target = null;
                return;
            }

            _isInRange = WithinAttackDistance();

            if (!_isInRange)
            {
                _ai.UpdateTargetPosition(_target.transform.position);
            }
            else
            {
                _ai.Cancel();
                AttackBehaviour();
            }
        }
    }

    public bool WithinAttackDistance()
    {
        return _isInRange = Vector3.Distance(transform.position, _target.transform.position) < _weaponRange;
    }

    private void AttackBehaviour()
    {
        if (_anim.GetBool("move") == false)
        {
            _ai.RotateFacingTowardsTarget(_target.transform);
            if (_timeSinceLastAttack >= _timeBetweenAttacks)
            {
                _anim.ResetTrigger("stopAttack");
                _anim.SetTrigger("attacking");
                _isAttacking = true;
                _timeSinceLastAttack = 0f;
            }
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
        _target = null;
    }

    public void Attack(Transform warrior)
    {
        if(warrior != null)
        { 
            _target = warrior.GetComponent<Health>();
        }
        else
        {
            _target = null;
        }
    }

    void Hit()
    {
        if (_target != null)
        {
            if (WithinAttackDistance())
            {//target in range
                _target.TakeDamage(damage);

                _posWarrior = _target.transform.position;
                _posWarrior.y = 0.8f;
                _bloodParticles.transform.position = _posWarrior;
                _bloodParticles.GetComponent<ParticleSystem>().Play();
            }
        }
    }
}
