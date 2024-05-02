using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public bool _allowMouseMovement = false;

    [SerializeField] private float _panSpeed = 20f;
    private float _panBorderThickness = 20f;

    private float _scrollSpeed = 10f;

    [SerializeField] float _minY = 15f;
    [SerializeField] float _maxY = 25f;

    [SerializeField] float _minX = 0f;
    [SerializeField] float _maxX = 40f;

    [SerializeField] float _minZ = 0f;
    [SerializeField] float _maxZ = 40f;


    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (_allowMouseMovement)
        {
            if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - _panBorderThickness)
            {
                pos.x += (_panSpeed * Time.deltaTime) / 2;
            }
            if (Input.GetKey("s") || Input.mousePosition.y <= _panBorderThickness)
            {
                pos.x -= (_panSpeed * Time.deltaTime) / 2;
            }
            if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - _panBorderThickness)
            {
                pos.z -= (_panSpeed * Time.deltaTime) / 2;
            }
            if (Input.GetKey("a") || Input.mousePosition.x <= _panBorderThickness)
            {
                pos.z += (_panSpeed * Time.deltaTime) / 2;
            }
        }
        else
        {
            if (Input.GetKey("w"))
            {
                pos.x += (_panSpeed * Time.deltaTime) / 2;
            }
            if (Input.GetKey("s"))
            {
                pos.x -= (_panSpeed * Time.deltaTime) / 2;
            }
            if (Input.GetKey("d"))
            {
                pos.z -= (_panSpeed * Time.deltaTime) / 2;
            }
            if (Input.GetKey("a"))
            {
                pos.z += (_panSpeed * Time.deltaTime) / 2;
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * _scrollSpeed * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
        pos.z = Mathf.Clamp(pos.z, _minZ, _maxZ);
        pos.y = Mathf.Clamp(pos.y, _minY, _maxY);

        transform.position = pos;
    }
}
