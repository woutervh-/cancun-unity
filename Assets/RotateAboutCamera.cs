﻿using System;
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
    public float radius = 2.0f;
    
    private Vector3 dragOrigin = Vector3.zero;
    private bool isDragging = false;
    private float latitude = 0.0f;
    private float longitude = 0.0f;
    
    void Start()
    {
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

        if (isDragging)
        {
            Vector3 position = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            dragOrigin = Input.mousePosition;
            latitude -= position.y * speedVertical;
            longitude -= position.x * speedHorizontal;

            UpdateValues();
        }
    }

    void UpdateValues()
    {
        transform.position = CalculatePosition();
        transform.rotation = CalculateRotation();
        latitudeText.text = "Latitude: " + latitude;
        longitudeText.text = "Longitude: " + longitude;
    }

    Vector3 CalculatePosition()
    {
        return CalculateRotation() * Vector3.back * radius;
    }

    Quaternion CalculateRotation()
    {
        return Quaternion.Euler(latitude, -longitude, 0);
    }
}
