using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Earth : MonoBehaviour
{
    public Text areaText;

    [Range(0, 6)]
    public int subdivisions = 4;

    private static int tileSize = 256;

    private string url = "https://api.tomtom.com/map/1/tile/basic/main/0/0/0.png?key=zu3b4a3g2x87pc2sdqeb6put";

    private string[] urls = new string[] {
        "https://api.tomtom.com/map/1/tile/basic/main/1/0/0.png?key=zu3b4a3g2x87pc2sdqeb6put",
        "https://api.tomtom.com/map/1/tile/basic/main/1/1/0.png?key=zu3b4a3g2x87pc2sdqeb6put",
        "https://api.tomtom.com/map/1/tile/basic/main/1/0/1.png?key=zu3b4a3g2x87pc2sdqeb6put",
        "https://api.tomtom.com/map/1/tile/basic/main/1/1/1.png?key=zu3b4a3g2x87pc2sdqeb6put"
    };

    // Use this for initialization
    private IEnumerator Start()
    {
        GenerateMesh();
        yield return GenerateTexture();
    }

    // Update is called once per frame
    private void Update()
    {
        float unitPixelRadius = (Camera.main.projectionMatrix * Vector3.right).x * Camera.main.pixelWidth;
        float earthPixelRadius = unitPixelRadius / 2;
        float earthPixelArea = Mathf.PI * earthPixelRadius * earthPixelRadius;
        int tilesNeeded = Mathf.CeilToInt(earthPixelArea / (tileSize * tileSize));
        float zoomLevel = Mathf.Log(tilesNeeded, 2);

        areaText.text = "Area: " + earthPixelArea + "p² (tiles: " + tilesNeeded + ", zoom level: " + zoomLevel + ")";
    }

    public IEnumerator GenerateTexture()
    {
        //WWW www = new WWW(url);
        //yield return www;
        //Texture2D texture = new Texture2D(tileSize, tileSize, TextureFormat.RGB24, false);
        //texture.filterMode = FilterMode.Trilinear;
        //texture.anisoLevel = 9;
        //www.LoadImageIntoTexture(texture);
        //GetComponent<Renderer>().material.mainTexture = texture;

        WWW[] WWWs = new WWW[urls.Length];
        for (int i = 0; i < urls.Length; i++)
        {
            string url = urls[i];
            WWW tileWWW = new WWW(url);
            WWWs[i] = tileWWW;
        }

        foreach (WWW tileWWW in WWWs)
        {
            yield return tileWWW;
        }

        Texture2DArray mainTexture = new Texture2DArray(tileSize, tileSize, urls.Length, TextureFormat.RGBA32, false);

        for (int i = 0; i < urls.Length; i++)
        {
            WWW tileWWW = WWWs[i];
            Texture2D tileTexture = new Texture2D(tileSize, tileSize, TextureFormat.RGB24, false);
            // tileTexture.filterMode = FilterMode.Trilinear;
            // tileTexture.anisoLevel = 9;
            // tileTexture.wrapMode = TextureWrapMode.Clamp;
            tileWWW.LoadImageIntoTexture(tileTexture);
            mainTexture.SetPixels(tileTexture.GetPixels(), i);
        }
        mainTexture.wrapMode = TextureWrapMode.Clamp;
        mainTexture.anisoLevel = 9;
        mainTexture.filterMode = FilterMode.Trilinear;
        mainTexture.Apply();
        GetComponent<Renderer>().material.SetTexture("_TexArray", mainTexture);
    }

    public void GenerateMesh()
    {
        Mesh mesh = EarthCreator.Create(subdivisions, 0.5f);
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
