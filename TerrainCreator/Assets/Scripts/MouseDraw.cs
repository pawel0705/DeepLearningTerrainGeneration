using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseDraw : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsEraser = false;

    public bool IsConstantDrawPrediction = true;

    public bool IsConstantTextureUpdate = true;

    public Color32 penColour = new Color32(0, 0, 0, 255);

    public Color32 backroundColour = new Color32(0, 0, 0, 255);

    [Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public int startingHeight;
        public int overlap;
    }

    public SplatHeights[] splatHeights;

    [SerializeField]
    private Canvas HostCanvas;

    [SerializeField]
    private PredictionClient client;

    [Range(1, 20)]
    public int penRadius = 1;

    private Image penPointer;

    [SerializeField]
    private Terrain terrain;

    private bool _isInFocus = false;

    private byte[] output_tmp;

    private bool predicted = false;

    private bool stillDrawing = false;

    [SerializeField]
    private Image penPointer1, penPointer2, penPointer3, penPointer4, penPointer5, penPointer6, penPointer7, penPointer8, penPointer9, penPointer10;

    [SerializeField]
    private GameObject sketchTexture, heightmapTexture;

    private int basicSize = 512;

    public bool IsInFocus
    {
        get => _isInFocus;
        private set
        {
            if (value != _isInFocus)
            {
                this._isInFocus = value;
                this.TogglePenPointerVisibility(value);
            }
        }
    }

    private RawImage m_image;

    private Vector2? m_lastPos;

    public RawImage m_image_heightmap;

    void Start()
    {
        this.Init();
    }

    void Update()
    {
        var pos = Input.mousePosition;

        if (this.IsInFocus == true)
        {
            this.SetPenPointerPosition(pos);

            if (Input.GetMouseButton(0))
            {
                this.WritePixels(pos);

                if (this.IsConstantDrawPrediction == true)
                {
                    this.stillDrawing = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            this.m_lastPos = null;
        }

        if (this.predicted == true)
        {
            if (this.stillDrawing == false)
            {
                this.RebuildTerrain();
            }

            if (this.stillDrawing == true && this.IsConstantDrawPrediction == true)
            {
                this.stillDrawing = false;
                this.Predict();
            }

            this.RepaintHeightmap();
        }
    }

    private void OnEnable()
    {
        this.m_image = transform.GetComponent<RawImage>();
        this.TogglePenPointerVisibility(false);
    }

    private void OnDisable()
    {

    }

    private void RepaintHeightmap()
    {
        var tex2d = new Texture2D(this.basicSize, this.basicSize, TextureFormat.RGB24, false);
        var startPoint = 0;

        var colorsTmpArray = new Color[this.basicSize * this.basicSize];

        for (var x = this.basicSize - 1; x > 0; x--)
        {
            for (var y = 0; y < this.basicSize; y++)
            {
                colorsTmpArray[y + (x * this.basicSize)] = new Color(this.output_tmp[startPoint] / 255.0f, this.output_tmp[startPoint + 1] / 255.0f, this.output_tmp[startPoint + 2] / 255.0f);
                startPoint += 3;
            }
        }

        tex2d.SetPixels(colorsTmpArray);
        tex2d.Apply();

        this.m_image_heightmap.texture = tex2d;

        this.predicted = false;
    }

    private void RebuildTerrain()
    {
        var startPoint = 0;
        var heights = new float[this.basicSize, this.basicSize];

        for (var x = this.basicSize - 1; x > 0; x--)
        {
            for (var y = 0; y < this.basicSize; y++)
            {
                var colTmp = new Color(this.output_tmp[startPoint] / 255.0f, this.output_tmp[startPoint + 1] / 255.0f, this.output_tmp[startPoint + 2] / 255.0f);

                if (y > 1)
                {
                    heights[x, y] = colTmp.grayscale;
                }

                startPoint += 3;
            }
        }

        this.terrain.terrainData.SetHeights(0, 0, heights);

        if (this.IsConstantTextureUpdate == true)
        {
            this.RepaintTerrain();
        }
    }

    private void Init()
    {
        var tex = new Texture2D(this.basicSize, this.basicSize, TextureFormat.RGBA32, false);

        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                tex.SetPixel(i, j, this.backroundColour);
            }
        }

        tex.Apply();
        this.m_image.texture = tex;
    }

    private void WritePixels(Vector2 pos)
    {
        var mainTex = this.m_image.texture;

        var tex2d = new Texture2D(this.basicSize, this.basicSize, TextureFormat.RGBA32, false);

        var curTex = RenderTexture.active;
        var renTex = new RenderTexture(this.basicSize, this.basicSize, 32);

        Graphics.Blit(mainTex, renTex);
        RenderTexture.active = renTex;

        tex2d.ReadPixels(new Rect(0, 0, this.basicSize, this.basicSize), 0, 0);

        var col = this.IsEraser ? this.backroundColour : this.penColour;

        var positions = this.m_lastPos.HasValue ? this.GetLinearPositions(this.m_lastPos.Value, pos) : new List<Vector2>() { pos };

        foreach (var position in positions)
        {
            var pixels = GetNeighbouringPixels(new Vector2(this.basicSize, this.basicSize), position, this.penRadius);

            if (pixels.Count > 0)
            {
                foreach (var p in pixels)
                {
                    tex2d.SetPixel((int)p.x, (int)p.y, col);
                }
            }
        }


        tex2d.Apply();

        RenderTexture.active = curTex;
        renTex.Release();
        Destroy(renTex);
        Destroy(mainTex);
        curTex = null;
        renTex = null;
        mainTex = null;

        this.m_image.texture = tex2d;
        this.m_lastPos = pos;

        if (this.IsConstantDrawPrediction == true)
        {
            Predict();
        }
    }

    public void ClearTexture()
    {
        var tex2d = new Texture2D(this.basicSize, this.basicSize, TextureFormat.RGB24, false);

        for (int i = 0; i < tex2d.width; i++)
        {
            for (int j = 0; j < tex2d.height; j++)
            {
                tex2d.SetPixel(i, j, backroundColour);
            }
        }

        tex2d.Apply();
        this.m_image.texture = tex2d;
        this.m_image_heightmap.texture = tex2d;

        var heights = new float[this.basicSize, this.basicSize];

        this.terrain.terrainData.SetHeights(0, 0, heights);
    }

    private List<Vector2> GetNeighbouringPixels(Vector2 textureSize, Vector2 position, int brushRadius)
    {
        var pixels = new List<Vector2>();

        for (int i = -brushRadius; i < brushRadius; i++)
        {
            for (int j = -brushRadius; j < brushRadius; j++)
            {
                var pxl = new Vector2(position.x + i, position.y + j);
                if (pxl.x > 0 && pxl.x < textureSize.x && pxl.y > 0 && pxl.y < textureSize.y)
                {
                    pixels.Add(pxl);
                }
            }
        }

        return pixels;
    }

    private List<Vector2> GetLinearPositions(Vector2 firstPos, Vector2 secondPos, int spacing = 2)
    {
        var positions = new List<Vector2>();

        var dir = secondPos - firstPos;

        if (dir.magnitude <= spacing)
        {
            positions.Add(secondPos);
            return positions;
        }

        for (int i = 0; i < dir.magnitude; i += spacing)
        {
            var v = Vector2.ClampMagnitude(dir, i);
            positions.Add(firstPos + v);
        }

        positions.Add(secondPos);
        return positions;
    }

    public void SetPenColour(Color32 color, int number)
    {
        this.penColour = color;

        switch (number)
        {
            case 1:
                this.penPointer = this.penPointer1;
                break;
            case 2:
                this.penPointer = this.penPointer2;
                break;
            case 3:
                this.penPointer = this.penPointer3;
                break;
            case 4:
                this.penPointer = this.penPointer4;
                break;
            case 5:
                this.penPointer = this.penPointer5;
                break;
            case 6:
                this.penPointer = this.penPointer6;
                break;
            case 7:
                this.penPointer = this.penPointer7;
                break;
            case 8:
                this.penPointer = this.penPointer8;
                break;
            case 9:
                this.penPointer = this.penPointer9;
                break;
            case 10:
                this.penPointer = this.penPointer10;
                break;
            default:
                this.penPointer = this.penPointer1;
                break;
        }
    }

    public void SetPenRadius(int radius) => penRadius = radius;

    private void SetPenPointerSize()
    {
        var rt = this.penPointer.rectTransform;
        rt.sizeDelta = new Vector2(25, 25);
    }

    private void SetPenPointerPosition(Vector2 pos)
    {
        this.penPointer.transform.position = pos;
    }

    private void TogglePenPointerVisibility(bool isVisible)
    {
        if (isVisible)
        {
            SetPenPointerSize();
        }

        this.penPointer.gameObject.SetActive(isVisible);
        Cursor.visible = !isVisible;
    }

    public void OnPointerEnter(PointerEventData eventData) => IsInFocus = true;

    public void OnPointerExit(PointerEventData eventData) => IsInFocus = false;

    public void Predict()
    {
        var mainTex = this.m_image.texture;
        var tex2d = new Texture2D(mainTex.width, mainTex.height, TextureFormat.RGB24, false);

        var curTex = RenderTexture.active;
        var renTex = new RenderTexture(mainTex.width, mainTex.height, 32);

        Graphics.Blit(mainTex, renTex);
        RenderTexture.active = renTex;

        tex2d.ReadPixels(new Rect(0, 0, mainTex.width, mainTex.height), 0, 0);

        tex2d.Apply();

        RenderTexture.active = curTex;
        Destroy(renTex);
        curTex = null;
        renTex = null;
        mainTex = null;

        var input = tex2d.EncodeToPNG();

        this.client.Predict(input, output =>
        {
            output_tmp = new byte[output.Length];
            Array.Copy(output, 0, this.output_tmp, 0, output.Length);
            predicted = true;

        }, error =>
        {

        });
    }

    public void ServerReconnect()
    {
        this.client.ServerReconnect();
    }

    public void ExportSketch(string targetDirectory, string fileName)
    {
        var dt = DateTime.Now.ToString("yyMMdd_hhmmss");
        fileName += $"_s_{dt}";

        targetDirectory = Path.Combine(targetDirectory, "TerrainAI");

        var mainTex = this.m_image.texture;
        var tex2d = new Texture2D(mainTex.width, mainTex.height, TextureFormat.RGBA32, false);

        var curTex = RenderTexture.active;
        var renTex = new RenderTexture(mainTex.width, mainTex.height, 32);

        Graphics.Blit(mainTex, renTex);
        RenderTexture.active = renTex;

        tex2d.ReadPixels(new Rect(0, 0, mainTex.width, mainTex.height), 0, 0);

        tex2d.Apply();

        RenderTexture.active = curTex;
        Destroy(renTex);
        curTex = null;
        renTex = null;
        mainTex = null;

        var png = tex2d.EncodeToPNG();

        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        var fp = Path.Combine(targetDirectory, fileName + ".png");

        if (File.Exists(fp))
        {
            File.Delete(fp);
        }

        File.WriteAllBytes(fp, png);

        System.Diagnostics.Process.Start(targetDirectory);
    }

    public void ExportHeightmap(string targetDirectory, string fileName)
    {
        var dt = DateTime.Now.ToString("yyMMdd_hhmmss");
        fileName += $"_h_{dt}";

        targetDirectory = Path.Combine(targetDirectory, "TerrainAI");

        var mainTex = this.m_image_heightmap.texture;
        var tex2d = new Texture2D(mainTex.width, mainTex.height, TextureFormat.RGBA32, false);

        var curTex = RenderTexture.active;
        var renTex = new RenderTexture(mainTex.width, mainTex.height, 32);

        Graphics.Blit(mainTex, renTex);
        RenderTexture.active = renTex;

        tex2d.ReadPixels(new Rect(0, 0, mainTex.width, mainTex.height), 0, 0);

        tex2d.Apply();

        RenderTexture.active = curTex;
        Destroy(renTex);
        curTex = null;
        renTex = null;
        mainTex = null;

        var png = tex2d.EncodeToPNG();

        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        var fp = Path.Combine(targetDirectory, fileName + ".png");

        if (File.Exists(fp))
        {
            File.Delete(fp);
        }

        File.WriteAllBytes(fp, png);

        System.Diagnostics.Process.Start(targetDirectory);
    }

    private void RepaintTerrain()
    {
        this.terrain.terrainData.alphamapResolution = this.terrain.terrainData.heightmapResolution - 1;

        var splatmapData = new float[this.terrain.terrainData.alphamapWidth,
             this.terrain.terrainData.alphamapHeight,
             this.terrain.terrainData.alphamapLayers];

        for (int y = 0; y < this.terrain.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < this.terrain.terrainData.alphamapWidth; x++)
            {
                var terrainHeight = this.terrain.terrainData.GetHeight(y, x);
                var splat = new float[this.splatHeights.Length];

                for (int i = 0; i < this.splatHeights.Length; i++)
                {
                    float thisNoise = Map(Mathf.PerlinNoise(x * 0.04f, y * 0.04f), 0, 1, 0.5f, 1);

                    float thisHeightStart = this.splatHeights[i].startingHeight * thisNoise
                        - this.splatHeights[i].overlap * thisNoise;
                    float nextHeightStart = 0;

                    if (i != this.splatHeights.Length - 1)
                    {
                        nextHeightStart = this.splatHeights[i + 1].startingHeight * thisNoise
                            + this.splatHeights[i + 1].overlap * thisNoise;
                    }

                    if (i == this.splatHeights.Length - 1 && terrainHeight >= thisHeightStart)
                    {
                        splat[i] = 1;
                    }
                    else if (terrainHeight >= thisHeightStart && terrainHeight <= nextHeightStart)
                    {
                        splat[i] = 1;
                    }
                }

                this.Normalize(splat);

                for (int j = 0; j < this.splatHeights.Length; j++)
                {
                    splatmapData[x, y, j] = splat[j];
                }
            }
        }

        this.terrain.terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    public void UpdateTexture()
    {
        this.RepaintTerrain();
    }

    private void Normalize(float[] v)
    {
        float total = 0;
        for (int i = 0; i < v.Length; i++)
        {
            total += v[i];
        }

        for (int i = 0; i < v.Length; i++)
        {
            v[i] /= total;
        }
    }

    private float Map(float value, float sMin, float sMax, float mMin, float mMax)
    {
        return (value - sMin) * (mMax - mMin) / (sMax - sMin) + mMin;
    }
}
