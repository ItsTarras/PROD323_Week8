using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    //UIManager _uiManager;

    [SerializeField] Image _healthBar;

    [SerializeField] GameObject _canvasBar;

    [SerializeField] ArmyManager _armyManager;
    [SerializeField] float _maxHealth = 100f;
    [SerializeField] float _health;

    [SerializeField] Camera _camera;
    Animator _anim;


    bool _isDead = false;

    public bool isDead { get { return _isDead; } }


    void UpdateHealthbar()
    {
        float fill = _health / _maxHealth;
        if (_healthBar != null)
        {
            
            _healthBar.fillAmount = fill;
        }
    }

    void Start()
    {
        //_healthBar = GetComponent<Image>();
        //_uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _health = _maxHealth;
        
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _armyManager = GameObject.Find("Army Manager").GetComponent<ArmyManager>();
        _anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        _health = Mathf.Max(_health - damage, 0);
    }

    public void Cancel()
    {
        _isDead = false;
    }

    public void Die()
    {
        if (_isDead) return;

        _anim.SetTrigger("die");
        _isDead = true;
        this.gameObject.layer = 2;

        gameObject.tag = "Untagged";

        if (gameObject.GetComponent<Enemy>() != null)
        {
            Destroy(gameObject.GetComponent<Enemy>());
            Destroy(gameObject.GetComponent<AIController>());
            Destroy(gameObject, 2f);
        }
        else if (gameObject.GetComponent<PlayerUnitFSM>() != null)
        {
            _camera.GetComponent<UnitSelection>().RemoveWarrior(gameObject);
            _armyManager.RemoveThis(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        _canvasBar.gameObject.transform.LookAt(_camera.transform);
        UpdateHealthbar();

        if (_health <= 0 && !_isDead)
        {
            Die();
        }
    }
}
