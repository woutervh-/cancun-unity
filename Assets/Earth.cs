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

    private static string urlTemplate = "https://api.tomtom.com/map/1/tile/basic/main/{z}/{x}/{y}.png?key=zu3b4a3g2x87pc2sdqeb6put";

    private int oldZoomLevel = 0;

    // Use this for initialization
    private IEnumerator Start()
    {
        GenerateMesh();
        yield return GenerateTexture(0);
    }

    // Update is called once per frame
    private void Update()
    {
        float unitPixelRadius = (Camera.main.projectionMatrix * Vector3.right).x * Camera.main.pixelWidth;
        float earthPixelRadius = unitPixelRadius / 2;
        float earthPixelArea = Mathf.PI * earthPixelRadius * earthPixelRadius;
        int tilesNeeded = Mathf.CeilToInt(earthPixelArea / (tileSize * tileSize));
        float zoom = Mathf.Log(tilesNeeded, 2);
        int zoomLevel = Mathf.FloorToInt(zoom);

        areaText.text = "Area: " + earthPixelArea + "p² (tiles: " + tilesNeeded + ", zoom level: " + zoom + ")";

        if(zoomLevel != oldZoomLevel)
        {
            Debug.Log("New zoom level " + zoomLevel);
            oldZoomLevel = zoomLevel;
            StartCoroutine(GenerateTexture(zoomLevel));
        }
    }

    public IEnumerator GenerateTexture(int zoomLevel)
    {
        int tileCount = Mathf.FloorToInt(Mathf.Pow(2, zoomLevel));
        string[] urls = new string[tileCount * tileCount];

        for (int x = 0; x < tileCount; x++)
        {
            for (int y = 0; y < tileCount; y++)
            {
                urls[y * tileCount + x] = urlTemplate.Replace("{z}", zoomLevel.ToString()).Replace("{x}", x.ToString()).Replace("{y}", y.ToString());
            }
        }

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

        Texture2DArray textureArray = new Texture2DArray(tileSize, tileSize, urls.Length, TextureFormat.RGBA32, false);

        for (int i = 0; i < urls.Length; i++)
        {
            WWW tileWWW = WWWs[i];
            Texture2D tileTexture = new Texture2D(tileSize, tileSize, TextureFormat.RGB24, false);
            tileWWW.LoadImageIntoTexture(tileTexture);
            textureArray.SetPixels(tileTexture.GetPixels(), i);
        }
        textureArray.wrapMode = TextureWrapMode.Clamp;
        textureArray.anisoLevel = 9;
        textureArray.filterMode = FilterMode.Trilinear;
        textureArray.Apply();
        GetComponent<Renderer>().material.SetInt("_TileCount", tileCount);
        GetComponent<Renderer>().material.SetTexture("_Textures", textureArray);
    }

    public void GenerateMesh()
    {
        Mesh mesh = EarthCreator.Create(subdivisions, 0.5f);
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
