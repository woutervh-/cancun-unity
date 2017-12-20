using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateAboutCamera : MonoBehaviour
{
    public Text latitudeText;
    public Text longitudeText;
    public Text zoomText;
    public float speedHorizontal = 180f;
    public float speedVertical = 90f;
    public float minFieldOfView = 0f;
    public float maxFieldOfView = 90f;

    private static float sqrt2 = Mathf.Sqrt(2f);

    private Vector3 dragOrigin = Vector3.zero;
    private bool isDragging = false;
    private float latitude = 0f;
    private float longitude = 0f;
    private float zoom = 1f;
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

            // Clamp latitude to -90, 90
            latitude -= position.y * speedVertical * zoom;
            latitude = Mathf.Clamp(latitude, -90f, 90f);

            // Wrap longitude in -180, 180
            longitude -= position.x * speedHorizontal * zoom;
            longitude += 180;
            longitude %= 360;
            longitude += 360;
            longitude %= 360;
            longitude -= 180;

            update = true;
        }

        if (Input.mouseScrollDelta.y < 0f)
        {
            zoom *= sqrt2;
            zoom = Mathf.Min(zoom, 1f);
            update = true;
        }
        else if (Input.mouseScrollDelta.y > 0f)
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
        zoomText.text = "Zoom: " + zoom;
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
        return Quaternion.Euler(latitude, -longitude, 0f);
    }
}
