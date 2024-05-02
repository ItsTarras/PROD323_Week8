using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class UnitSelection : MonoBehaviour
{
    #region
    [SerializeField] Color boxColor;
    [SerializeField] Color borderColor;

    public bool useFormation = false;
    [SerializeField] FormationSelector formation; 
    public List<GameObject> armyList = new List<GameObject>();
    public GameObject target;

    private Transform _selection = null;
    private string enemyTag   = "Enemy"  ;
    private string armyTag    = "Army"   ;
    private string terrainTag = "Terrain";

    RaycastHit theObject;
    public static GameObject selectedObject;
    bool _isDragging = false;

    Vector3 mousePosition;
    #endregion

    // Update is called once per frame
    void Update()
    {   
        //**** DRAWING THE SELECTION BOX ****

        // Take the mouse position. This is the first left-click.
        if (Input.GetMouseButtonDown(0)) 
        {
            mousePosition = Input.mousePosition;
        }

        // This will detect if the left mouse button is being held
        if (Input.GetMouseButton(0))
        {
            // If there's a significant distance between your first left-click and
            // the current mouse position, then the player is doing a mouse drag action
            if ((mousePosition - Input.mousePosition).magnitude > 40)
            {
                _isDragging = true;
            }
        }

        // 
        if (Input.GetMouseButtonUp(0))
        {
            if (_isDragging == false) 
            {
                CastRaycast();

                //After casting a ray, determine if there is a current selection
                // and if the second left-click selected the terrain, a unit or an enemy
                if (_selection != null)
                {
                    //If second left-click selected the terrain, then move the units to that point
                    if (_selection.tag == terrainTag)
                    {
                        //Make this the target point
                        ResetArmyEnemy();
                        if(!useFormation)
                            UpdateArmyTarget(theObject.point);
                        else
                            UpdateArmyFormation(theObject.point);
                    }  

                    // If the second left-click selected a unit, then select that unit and add it to the army
                    // but if the second left-click is coupled with a left shift, then remove the unit from 
                    // the army.
                    if (_selection.tag == armyTag)
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            if (_selection.GetComponent<PlayerUnitControl>().IsSelected())
                            {
                                RemoveWarrior(_selection.gameObject);
                            }
                            else
                            {
                                SelectWarrior(_selection.gameObject);
                            }
                        }
                        else
                        {
                            UnselectWarriors();
                            SelectWarrior(_selection.gameObject);
                        }
                    }

                    // If an enemy is selected, then each unit in the army will attack said enemy
                    if (_selection.tag == enemyTag)
                    {
                        foreach (GameObject warrior in armyList)
                        {
                            warrior.GetComponent<PlayerUnitFSM>().Attack(_selection.transform);
                        }
                    }
                }
            }

            // If currently dragging the mouse over...
            else
            {
                // Unselect any unit from army, basically reset the selection
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    UnselectWarriors();
                }

                // Add all units within the selection area
                foreach (var warrior in GameObject.FindGameObjectsWithTag(armyTag))
                {
                    if (IsWithinSelectionBounds(warrior.transform))
                    {
                        if (armyList.Contains(warrior) == false)
                        {
                            SelectWarrior(warrior);
                        }
                    }
                }
                _isDragging = false;
            }
        }

        // If right-click then remove all units in army
        if (Input.GetMouseButtonDown(1))
        {
            UnselectWarriors();
        }
    }

    public void RemoveWarrior(GameObject warrior)
    {
        armyList.Remove(warrior);
        warrior.GetComponent<PlayerUnitControl>().Unselect();
        warrior.transform.Find("Highlight").gameObject.SetActive(false);
    }

    private void SelectWarrior(GameObject warrior)
    {
        armyList.Add(warrior);
        warrior.GetComponent<PlayerUnitControl>().Select();
        warrior.transform.Find("Highlight").gameObject.SetActive(true);
    }
    
    private void UnselectWarriors()
    {
        foreach( GameObject warrior in armyList)
        {
            warrior.transform.Find("Highlight").gameObject.SetActive(false);
            warrior.GetComponent<PlayerUnitControl>().Unselect();
        }
        armyList.Clear();
    }

    private void ResetArmyEnemy()
    {
        foreach (GameObject warrior in armyList)
        {
            warrior.GetComponent<PlayerUnitFSM>().Cancel();
        }
    }

    private void UpdateArmyTarget(Vector3 position)
    {
        foreach (GameObject warrior in armyList)
        {
            warrior.GetComponent<PlayerUnitControl>().StartUpdateTargetPosition(position);
        }
    }

    private bool IsWithinSelectionBounds(Transform transform)
    {
        if (!_isDragging)
        {
            return false;
        }
        var camera = Camera.main;
        var viewportBounds = ScreenHelper.GetViewportBounds(camera, mousePosition, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(transform.position));
    }

    private void CastRaycast()
    {

        if(Input.mousePosition.y < 200) //if selection on the menu then ignore raycast
        {
            return;
        }

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        //// Bit shift the index of the layer (5) to get a bit mask
        //int layerMask = 1 << 5;
        //// This would cast rays only against colliders in layer 5.
        //// But instead we want to collide against everything except layer 5. The ~ operator does this, it inverts a bitmask.   
        //layerMask = ~layerMask;

        if (Physics.Raycast(camRay, out theObject, 100 /*, layerMask*/))
        {
            selectedObject = theObject.transform.gameObject;
            Transform selection = theObject.transform;

            if (selection.CompareTag(armyTag) 
            || (selection.CompareTag(terrainTag) && _isDragging == false) 
            || (selection.CompareTag(enemyTag) && _isDragging == false) )
            {
                _selection = selection;
            }
        }
    }

    private void OnGUI()
    {
        if (_isDragging)
        {
            var rect = ScreenHelper.GetScreenRect(mousePosition, Input.mousePosition);
            ScreenHelper.DrawScreenRect(rect, boxColor);
            ScreenHelper.DrawScreenRectBorder(rect, 1, borderColor);
        }
    }
    private void UpdateArmyFormation(Vector3 position)
    {
        if(armyList.Count > 0)
        {
            Transform leader = armyList[0].transform;
            int index = 0;
            float zOffset = 0f;
            foreach (GameObject warrior in armyList)
            {
                Vector3 pos = (position-leader.position) + formation.GetFormation(leader, index, zOffset, armyList.Count);
                warrior.GetComponent<PlayerUnitControl>().StartUpdateTargetPosition(pos);
                index++;
            }
        }
    }

    public void ForceUpdateFormation()
    {
        if(armyList.Count > 0)
        {
            Transform leader = armyList[0].transform;
            int index = 0;
            float zOffset = 0f;
            foreach (GameObject warrior in armyList)
            {
                Vector3 pos = formation.GetFormation(leader, index, zOffset, armyList.Count);
                warrior.GetComponent<PlayerUnitControl>().StartUpdateTargetPosition(pos);
                index++;
            }
        }
    }
}
