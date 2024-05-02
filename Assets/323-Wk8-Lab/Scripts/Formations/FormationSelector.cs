using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FormationSelector : MonoBehaviour
{
    private enum FormationType { Arc, Circle, Column, Diamond, Echelon, Grid, Line, Row, Square, Triangle, V, Wedge}
    private FormationType fType;
    public UnityEvent FormationChanged;

    #region Arc
    [Header("Arc")]
    [SerializeField] float arcRadius = 5; //The radius of the arc
    [SerializeField] bool isConcave = true; //Is the arc concave
    private float arcTheta;
    #endregion

    #region Circle
    [Header("Circle")]
    [SerializeField] float circleRadius = 5;
    private float theta2;
    #endregion

    #region Column
    [Header("Column")]
    [SerializeField] Vector2 columnDistance = new Vector2(2, 2);
    [SerializeField] int columns = 2;
    #endregion

    #region Diamond
    [Header("Diamond")]
    [SerializeField] Vector2 diamondDistance = new Vector2(2, 2);
    [SerializeField] bool backPositionOffset = false;
    #endregion

    #region Echelon
    [Header("Echelon")]
    [SerializeField] Vector2 echelonDistance = new Vector2(2, 2);
    [SerializeField] bool echelonLeft = false;
    #endregion

    #region Grid
    [Header("Grid")]
    [SerializeField] Vector2 gridDistance = new Vector2(2, 2);
    [SerializeField] int gridRows = 3;
    #endregion

    #region Line
    [Header("Line")]
    [SerializeField] float lineDistance = 2;
    [SerializeField] bool lineLeft;
    #endregion

    #region Row
    [Header("Row")]
    [SerializeField] Vector2 rowDistance = new Vector2(2, 2);
    [SerializeField] int rows = 2;
    #endregion

    #region Square
    [Header("Square")]
    [SerializeField] float squareSideLength = 5;
    private int[] squareUnitsPerSide = new int[4];
    #endregion

    #region Triangle    
    [Header("Triangle")]
    [SerializeField] float triangleSideLength = 5;
    private int[] unitsPerSideTriangle = new int[3];
    #endregion

    #region V
    [Header("V")]
    [SerializeField] Vector2 vDistance = new Vector2(2, 2);
    #endregion

    #region Wedge
    [Header("Wedge")]
    [SerializeField] Vector2 separation8 = new Vector2(2, 2);
    [SerializeField] bool fill;
    private List<List<bool>> filledHoles = new List<List<bool>>();
    bool filledStuff = false;
    #endregion

    private int currentRow = 1;
    private int currentAgentsPerRow = 0;
    private int lastIndex;

    void Start()
    {
        // Unity Event will inform if there was a change 
        if(FormationChanged == null)
            FormationChanged = new UnityEvent();

        fillList();

    }

    public void SetFormation(int type)
    {
        //Reset the list after every formation call.
        clearFills();
        fillList();
        fType = (FormationType) type;
        FormationChanged.Invoke();
    }

    private void clearFills()
    {
        filledHoles.Clear();
    }

    private void fillList()
    {
        Debug.Log(lastIndex);
        int numRows = (int)((-1 + Mathf.Sqrt(8 * 8 + 1))) + 1;
        numRows /= 2;

        Debug.Log(numRows);

        //Fill out the rows in the list.
        for (int i = 0; i < numRows; i++)
        {
            filledHoles.Add(new List<bool>());
            //Each item in the row.
            for (int j = 0; j < i + 1; j++)
            {
                filledHoles[i].Add(false);
            }
        }

        Debug.Log("Number of Rows: " +  filledHoles.Count);

        for (int i =0;  i < filledHoles.Count; i++)
        {
            Debug.Log("Row " + i + " Count: " + filledHoles[i].Count);
        }
    }



    public Vector3 GetFormation(Transform leader, int index, float zOffset, int size)
    {
        // Calculate position of the unit relative to the leader
        Vector3 pos = Vector3.zero;
        switch (fType) {
            case FormationType.Arc:
                pos = Arc(leader, index, zOffset, size);
                break;
            case FormationType.Circle:
                pos = Circle(leader, index, zOffset, size);
                break;
            case FormationType.Column:
                pos = Column(leader, index, zOffset);
                break;
            case FormationType.Diamond:
                pos = Diamond(leader, index, zOffset);
                break;
            case FormationType.Echelon:
                pos = Echelon(leader, index, zOffset);
                break;
            case FormationType.Grid:
                pos = Grid(leader, index, zOffset);
                break;
            case FormationType.Line:
                pos = Line(leader, index, zOffset);
                break;    
            case FormationType.Row:
                pos = Row(leader, index, zOffset);
                break;
            case FormationType.Square:
                pos = Square(leader, index, zOffset, size);
                break;
            case FormationType.Triangle:
                pos = Triangle(leader, index, zOffset, size);
                break;               
            case FormationType.V:
                pos = V(leader, index, zOffset);
                break;
            case FormationType.Wedge:
                pos = Wedge(leader, index, zOffset);
                break;                                                                                                                                             
        }
        return pos;
    }


    private Vector3 Arc(Transform leader, int index, float zOffset, int size)
    {
        // Divide the circle by the total number of units in the army to get an angle
        float circleTheta = Mathf.PI / size;
        if (isConcave)
        {
            return leader.TransformPoint(-arcRadius * Mathf.Sin(circleTheta * index), 0, circleRadius * Mathf.Cos(circleTheta * index) - arcRadius + zOffset);

        }

        // Using polar coordinates, you can calculate the X and Z axis with the given radius and theta
        // Subtract the radius from the z position to prevent the agents from getting ahead of the leader
        return leader.TransformPoint(arcRadius * Mathf.Sin(circleTheta * index), 0, circleRadius * Mathf.Cos(circleTheta * index) - arcRadius + zOffset);
    }

    private Vector3 Circle(Transform leader, int index, float zOffset, int size)
    {
        // Divide the circle by the total number of units in the army to get an angle
        float circleTheta = 2 * Mathf.PI / size;

        // Using polar coordinates, you can calculate the X and Z axis with the given radius and theta
        // Subtract the radius from the z position to prevent the agents from getting ahead of the leader
        return leader.TransformPoint(circleRadius * Mathf.Sin(circleTheta * index), 0, circleRadius * Mathf.Cos(circleTheta * index) - circleRadius + zOffset);
    }

    private Vector3 Column(Transform leader, int index, float zOffset)
    {
        int columnNumber = index % columns;

        int row = index / columns;

        if (columnNumber == 0)
        {
            //We are behind the leader.
            return leader.TransformPoint(0, 0, columnDistance.y * row);
        }
        else
        {
            //If it is even.
            if (columnNumber % 2 == 0)
            {
                return leader.TransformPoint(-columnDistance.x * ((columnNumber - 1) / 2 + 1), 0, -columnDistance.y * row + zOffset);
            }
            else
            {
                return leader.TransformPoint(columnDistance.x * ((columnNumber - 1) / 2 + 1), 0, columnDistance.y * row + zOffset);
            }
        }
    }

    private Vector3 Diamond(Transform leader, int index, float zOffset)
    {
        return Vector3.zero;
    }

    private Vector3 Echelon(Transform leader, int index, float zOffset)
    {
        //Default to the right side.

        Vector2 calculated = (echelonDistance * index);

        float xOffset = echelonDistance.x / 2;
        float yOffset = echelonDistance.y / 2;

        if (echelonLeft)
        {
            xOffset *= -1;
            yOffset *= -1;
        }

        return leader.TransformPoint(-calculated.x + xOffset, 0, -calculated.y * yOffset + zOffset);


    }

    private Vector3 Grid(Transform leader, int index, float zOffset)
    {
        int row = index % gridRows;
        int column = index / gridRows;

        if (index != 0)
        {
            return leader.TransformPoint(gridDistance.x * column, 0, gridDistance.y * row + zOffset);
        }

        return leader.TransformPoint(0, 0, zOffset);
    }

    private Vector3 Line(Transform leader, int index, float zOffset)
    {
        // If lineLeft is true, then all units are to be placed on the left of the leader.
        // Calculate the Vector3 position of each unit. The Y value will always be 0 since units are only positioned within the X and Z axis.
        return leader.TransformPoint(lineDistance * index * (lineLeft ? 1 : -1), 0, zOffset);
    }


    private Vector3 Row(Transform leader, int index, float zOffset)
    {
        int columnNumber = index / rows;

        int row = index % rows;

        if (columnNumber == 0)
        {
            //We are behind the leader.
            return leader.TransformPoint(0, 0, columnDistance.y * row);
        }
        else
        {
            //If it is even.
            if (columnNumber % 2 == 0)
            {
                return leader.TransformPoint(-rowDistance.x * ((columnNumber - 1) / 2 + 1), 0, rowDistance.y * row + zOffset);
            }
            else
            {
                return leader.TransformPoint(rowDistance.x * ((columnNumber - 1) / 2 + 1), 0, rowDistance.y * row + zOffset);
            }
        }
    }

    private Vector3 Square(Transform leader, int index, float zOffset, int size)
    {
        if (index == 0)
            return leader.TransformPoint(0, 0, zOffset);

        // Calculate the number of units per side. This can be done outside of this function because the result will always be the same depending on the size. 
        for (int i = 0; i < 4; i++)
        {
            squareUnitsPerSide[i] = size / 4 + (size % 4 > i ? 1 : 0);
        }

        // Calculate the side of the current unit. 
        var side = index % 4;

        // Calculate the subset of the actual squareSideLength to be used for creating the coordinate of the current unit relative to the leader. 
        var lengthMultiplier = (index / 4) / (float)squareUnitsPerSide[side]; lengthMultiplier = 1 - (lengthMultiplier - (int)lengthMultiplier);

        // Calculate the coordinate for the current unit 
        if (side == 0)
        {
            // top side 
            return leader.TransformPoint(squareSideLength * lengthMultiplier, 0, zOffset);
        }
        else if (side == 1)
        {
            // right side 
            return leader.TransformPoint(squareSideLength, 0, -squareSideLength * (1 - lengthMultiplier) + zOffset);
        }
        else if (side == 2)
        {
            //Bottom side
            return leader.TransformPoint(squareSideLength * lengthMultiplier, 0, -squareSideLength + zOffset); 
        }
        else
        {
            // left side 
            return leader.TransformPoint(0, 0, -squareSideLength * lengthMultiplier + zOffset);
        }
    }

    private Vector3 Triangle(Transform leader, int index, float zOffset, int size)
    {
        if (index == 0)
            return leader.TransformPoint(0, 0, zOffset);

        // Calculate the number of units per side. This can be done outside of this function because the result will always be the same depending on the size. 
        for (int i = 0; i < 3; i++)
        {
            unitsPerSideTriangle[i] = size / 3 + (size % 3 > i ? 1 : 0);
        }

        // Calculate the side of the current unit. 
        var side = index % 3;

        // Calculate the subset of the actual squareSideLength to be used for creating the coordinate of the current unit relative to the leader. 
        var lengthMultiplier = (index / 3) / (float)unitsPerSideTriangle[side]; lengthMultiplier = 1 - (lengthMultiplier - (int)lengthMultiplier);

        //Equalateral triangles are of angle 60.
        float height = Mathf.Tan(60f * Mathf.Deg2Rad) * triangleSideLength / 2f;


        // Calculate the coordinate for the current unit 
        if (side == 0)
        {
            // right side 
            return leader.TransformPoint(triangleSideLength / 2 * lengthMultiplier, 0f, height * lengthMultiplier);
        }
        else if (side == 1)
        {
            //Bottom side
            return leader.TransformPoint(Mathf.Lerp(-triangleSideLength / 2, triangleSideLength / 2, lengthMultiplier), 0, zOffset + height);
        }
        else
        {
            // left side 
            return leader.TransformPoint(-triangleSideLength / 2 * lengthMultiplier, 0f, height * lengthMultiplier);
        }
    }

    private Vector3 V(Transform leader, int index, float zOffset)
    {
        if (index == 0)
        {
            return leader.TransformPoint(0, 0, zOffset);
        }

        bool isLeft = false;
        if (index % 2 == 0)
        {
            isLeft = true;
        }


        if (isLeft)
        {
            return leader.TransformPoint(vDistance.x * ((index - 1) / 2 + 1), 0, vDistance.y * (((index - 1) / 2) + 1) + zOffset);
        }
        else
        {
            return leader.TransformPoint(-vDistance.x * ((index - 1) / 2 + 1), 0, vDistance.y * (((index - 1) / 2) + 1) + zOffset);
        }
    }

    private Vector3 Wedge(Transform leader, int index, float zOffset)
    {
        bool isLeft = false;
        if (index % 2 == 0)
        {
            isLeft = true;
        }

        if (index == 0)
        {
            filledHoles[0][0] = (true);
            return leader.TransformPoint(0, 0, zOffset);
        }

        if (!fill)
        {
            


            if (isLeft)
            {
                return leader.TransformPoint(-separation8.x * ((index - 1) / 2 + 1), 0, -separation8.y * (((index - 1) / 2) + 1) + zOffset);
            }
            else
            {
                return leader.TransformPoint(separation8.x * ((index - 1) / 2 + 1), 0, -separation8.y * (((index - 1) / 2) + 1) + zOffset);
            }
        }
        else
        {
            //Start pyramid logic.
            for (int i = 0; i < filledHoles.Count; i++)
            {
                for (int j = 0; j < filledHoles[i].Count; j++)
                {
                    //Get the row value, and the column value, and apply transformations accordingly.
                    if (filledHoles[i][j] == false)
                    {
                        //set it to true, and then apply return transform point.
                        filledHoles[i][j] = true;

                        float midPoint = (filledHoles[i].Count - 1) / 2;

                        float multiplier = (j - midPoint) / 2;

                        //Check the column if we are to the left of the midpoint. If we are, we will use a different transformation?
                        if(isLeft)
                        {
                            return leader.TransformPoint(separation8.x * i, 0, (separation8.y + zOffset) * i * multiplier);
                        }
                        else
                        {
                            return leader.TransformPoint(separation8.x * i, 0, (separation8.y + zOffset) * i * multiplier - 0.5f);
                        }
                    }
                }
            }
            return leader.TransformPoint(separation8.x * ((index - 1) / 2 + 1), 0, -separation8.y * (((index - 1) / 2) + 1));
        }

    }
}
