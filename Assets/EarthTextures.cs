﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EarthTextures : MonoBehaviour
{
    public Text areaText;

    private string url = "https://api.tomtom.com/map/1/tile/basic/main/0/0/0.png?key=zu3b4a3g2x87pc2sdqeb6put";

    // Use this for initialization
    IEnumerator Start()
    {
        Texture2D tex = new Texture2D(256, 256, TextureFormat.RGB24, false);
        tex.filterMode = FilterMode.Trilinear;
        tex.anisoLevel = 9;
        WWW www = new WWW(url);
        yield return www;
        www.LoadImageIntoTexture(tex);
        GetComponent<Renderer>().material.mainTexture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        float unitPixelRadius = (Camera.main.projectionMatrix * Vector3.right).x * Camera.main.pixelWidth;
        float earthPixelRadius = unitPixelRadius / 2;
        float earthPixelArea = Mathf.PI * earthPixelRadius * earthPixelRadius;

        areaText.text = "Area: " + earthPixelArea;
    }
}
