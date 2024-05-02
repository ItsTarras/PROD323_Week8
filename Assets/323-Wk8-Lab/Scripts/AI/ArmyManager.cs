using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyManager : MonoBehaviour
{
    [SerializeField] private List<PlayerUnitControl> _army = new List<PlayerUnitControl>();
    //private List<Warrior> _currentArmySelected = new List<Warrior>();
    private List<AIController> _enemyList = new List<AIController>();

    bool _isUnderAttack = false;


    // Start is called before the first frame update
    void Start()
    {
        UpdateEnemyList();
        UpdateArmy();
    }

    public void UpdateArmy()
    {
        _army.Clear();
        GameObject[] warriors = GameObject.FindGameObjectsWithTag("Army");
        
        foreach (GameObject warrior in warriors)
        {
            AddNewWarriorToList(warrior.GetComponent<PlayerUnitControl>());
        }
    }

    void AddNewEnemyToList(AIController enemy)
    {
        _enemyList.Add(enemy);
    }

    public void UpdateEnemyList()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _enemyList.Clear();

        if (enemies.Length > 0)
        {
            foreach (GameObject enemy in enemies)
            {
                AddNewEnemyToList(enemy.GetComponent<AIController>());
            }
        }
    }

    public List<AIController> GetEnemyList()
    {
        UpdateEnemyList();
        return _enemyList;
    }

    public void ArmyNotAttacked()
    {
        _isUnderAttack = false;
    }

    public void ArmyAttacked()
    {
        _isUnderAttack = true;
    }

    public void RemoveThis(GameObject obj)
    {
        PlayerUnitControl warrior = obj.GetComponent<PlayerUnitControl>();
        _army.Remove(warrior);
        UpdateArmy();

        Destroy(obj.GetComponent<PlayerUnitFSM>());
        Destroy(obj.GetComponent<PlayerUnitControl>());
        Destroy(obj, 2f);

    }

    public List<PlayerUnitControl> GetArmyList()
    {
        return _army;
    }
    
    public void AddNewWarriorToList(PlayerUnitControl warrior)
    {
        _army.Add(warrior);
    }

    // Update is called once per frame

}
