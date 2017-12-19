using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateAboutCamera : MonoBehaviour
{
    public Text latitudeText;
    public Text longitudeText;
    public float speedHorizontal = 180.0f;
    public float speedVertical = 90.0f;
    public float minFieldOfView = 1.0f;
    public float maxFieldOfView = 90.0f;

    private static float sqrt2 = Mathf.Sqrt(2);

    private Vector3 dragOrigin = Vector3.zero;
    private bool isDragging = false;
    private float latitude = 0.0f;
    private float longitude = 0.0f;
    private float zoom = 1.0f;
    private Camera cameraObject;

    void Start()
    {
        cameraObject = GetComponent<Camera>();
        UpdateValues();
    }

    void LateUpdate()
    {
        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }
        else if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        bool update = false;

        if (isDragging)
        {
            Vector3 position = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            dragOrigin = Input.mousePosition;
            latitude -= position.y * speedVertical * zoom;
            longitude -= position.x * speedHorizontal * zoom;

            update = true;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            zoom *= sqrt2;
            update = true;
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            zoom /= sqrt2;
            update = true;
        }

        if (update)
        {
            UpdateValues();
        }
    }

    void UpdateValues()
    {
        transform.position = CalculatePosition();
        transform.rotation = CalculateRotation();
        cameraObject.fieldOfView = CalculateFieldOfView();
        latitudeText.text = "Latitude: " + latitude;
        longitudeText.text = "Longitude: " + longitude;
    }

    float CalculateFieldOfView()
    {
        return Mathf.Lerp(minFieldOfView, maxFieldOfView, zoom);
    }

    Vector3 CalculatePosition()
    {
        return CalculateRotation() * Vector3.back;
    }

    Quaternion CalculateRotation()
    {
        return Quaternion.Euler(latitude, -longitude, 0);
    }
}
