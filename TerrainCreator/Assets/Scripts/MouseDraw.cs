using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MouseDraw : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    [Tooltip("The Canvas which is a parent to this Mouse Drawing Component")]
    private Canvas HostCanvas;

    [SerializeField]
    private PredictionClient client;

    [Range(1, 20)]
    [Tooltip("The Pens Radius")]
    public int penRadius = 1;

    [Tooltip("The Pens Colour.")]
    public Color32 penColour = new Color32(0, 0, 0, 255);

    [Tooltip("The Drawing Background Colour.")]
    public Color32 backroundColour = new Color32(0, 0, 0, 255);

    [SerializeField]
    [Tooltip("Pen Pointer Graphic GameObject")]
    private Image penPointer;

    [SerializeField]
    private Terrain terrain;

    [Tooltip("Toggles between Pen and Eraser.")]
    public bool IsEraser = false;

    private bool _isInFocus = false;

    private byte[] output_tmp;
    /// <summary>
    /// Is this Component in focus.
    /// </summary>
    public bool IsInFocus
    {
        get => _isInFocus;
        private set
        {
            if (value != _isInFocus)
            {
                _isInFocus = value;
                TogglePenPointerVisibility(value);
            }
        }
    }

    private float m_scaleFactor = 10;
    private RawImage m_image;

    private Vector2? m_lastPos;

    private Texture2D tmp_text;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        m_image = transform.GetComponent<RawImage>();
        TogglePenPointerVisibility(false);
    }


    // Update is called once per frame
    void Update()
    {
        var pos = Input.mousePosition;

        if (IsInFocus)
        {
            SetPenPointerPosition(pos);

            if (Input.GetMouseButton(0))
                WritePixels(pos);
        }

        if (Input.GetMouseButtonUp(0))
            m_lastPos = null;
    }

    /// <summary>
    /// Initialisation logic.
    /// </summary>
    private void Init()
    {
        // Set scale Factor...
        m_scaleFactor = HostCanvas.scaleFactor * 2;

        // var tex = new Texture2D(Convert.ToInt32(Screen.width / m_scaleFactor), Convert.ToInt32(Screen.height / m_scaleFactor), TextureFormat.RGBA32, false);

        var tex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                tex.SetPixel(i, j, backroundColour);
            }
        }

        tex.Apply();
        m_image.texture = tex;
    }

    /// <summary>
    /// Writes the pixels to the Texture at the given ScreenSpace position.
    /// </summary>
    /// <param name="pos"></param>
    private void WritePixels(Vector2 pos)
    {
        var mainTex = m_image.texture;
        var tex2d = new Texture2D(mainTex.width, mainTex.height, TextureFormat.RGBA32, false);

        var curTex = RenderTexture.active;
        var renTex = new RenderTexture(mainTex.width, mainTex.height, 32);

        Graphics.Blit(mainTex, renTex);
        RenderTexture.active = renTex;

        tex2d.ReadPixels(new Rect(0, 0, mainTex.width, mainTex.height), 0, 0);

        var col = IsEraser ? backroundColour : penColour;

        var positions =  m_lastPos.HasValue ? GetLinearPositions(m_lastPos.Value, pos) : new List<Vector2>() { pos };

        foreach (var position in positions)
        {
            var pixels = GetNeighbouringPixels(new Vector2(mainTex.width, mainTex.height), position, penRadius);

            if (pixels.Count > 0)
                foreach (var p in pixels)
                    tex2d.SetPixel((int)p.x, (int)p.y, col);
        }

        tex2d.Apply();

        RenderTexture.active = curTex;
        renTex.Release();
        Destroy(renTex);
        Destroy(mainTex);
        curTex = null;
        renTex = null;
        mainTex = null;

        m_image.texture = tex2d;
        m_lastPos = pos;
    }

    /// <summary>
    /// Clears the Texture.
    /// </summary>
    [ContextMenu("Clear Texture")]
    public void ClearTexture()
    {
        var mainTex = m_image.texture;
        var tex2d = new Texture2D(mainTex.width, mainTex.height, TextureFormat.RGB24, false);

        /*
        for (int i = 0; i < tex2d.width; i++)
        {
            for (int j = 0; j < tex2d.height; j++)
            {
                tex2d.SetPixel(i, j, backroundColour);
            }
        }
        */

        var startPoint = 0;
        for (var x = 0; x < 256; x++)
        {
            for (var y = 0; y < 256; y++)
            {
                tex2d.SetPixel(x, y, new Color(output_tmp[startPoint] / 255.0f, output_tmp[startPoint + 1] / 255.0f, output_tmp[startPoint + 2] / 255.0f));
                startPoint += 3;
            }
        }

        tex2d.Apply();
        m_image.texture = tex2d;

        var heights = new float[256,256];
        for (int x = 0; x < 256; x++)
        {
            for (int y = 0; y < 256; y++)
            {
                heights[x, y] = tex2d.GetPixel(y, x).grayscale * 0.6f;
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);
    }

    /// <summary>
    /// Gets the neighbouring pixels at a given screenspace position.
    /// </summary>
    /// <param name="textureSize">The texture size or pixel domain.</param>
    /// <param name="position">The ScreenSpace position.</param>
    /// <param name="brushRadius">The Brush radius.</param>
    /// <returns>List of pixel positions.</returns>
    private List<Vector2> GetNeighbouringPixels(Vector2 textureSize, Vector2 position, int brushRadius)
    {
        var pixels = new List<Vector2>();

        for (int i = -brushRadius; i < brushRadius; i++)
        {
            for (int j = -brushRadius; j < brushRadius; j++)
            {
                var pxl = new Vector2(position.x + i, position.y + j);
                if (pxl.x > 0 && pxl.x < textureSize.x && pxl.y > 0 && pxl.y < textureSize.y)
                    pixels.Add(pxl);
            }
        }

        return pixels;
    }

    /// <summary>
    /// Interpolates between two positions with a spacing (default = 2)
    /// </summary>
    /// <param name="firstPos"></param>
    /// <param name="secondPos"></param>
    /// <param name="spacing"></param>
    /// <returns>List of interpolated positions</returns>
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

    /// <summary>
    /// Sets the Pens Colour.
    /// </summary>
    /// <param name="color"></param>
    public void SetPenColour(Color32 color) => penColour = color;

    /// <summary>
    /// Sets the Radius of the Pen.
    /// </summary>
    /// <param name="radius"></param>
    public void SetPenRadius(int radius) => penRadius = radius;

    /// <summary>
    /// Sets the Size of the Pen Pointer.
    /// </summary>
    private void SetPenPointerSize()
    {
        var rt = penPointer.rectTransform;
        rt.sizeDelta = new Vector2(penRadius * 5, penRadius * 5);
    }

    /// <summary>
    /// Sets the position of the Pen Pointer Graphic.
    /// </summary>
    /// <param name="pos"></param>
    private void SetPenPointerPosition(Vector2 pos)
    {
        penPointer.transform.position = pos;
    }

    /// <summary>
    /// Toggles the visibility of the Pen Pointer Graphic.
    /// </summary>
    /// <param name="isVisible"></param>
    private void TogglePenPointerVisibility(bool isVisible)
    {
        if (isVisible)
            SetPenPointerSize();

        penPointer.gameObject.SetActive(isVisible);
        Cursor.visible = !isVisible;
    }

    /// <summary>
    /// On Mouse Pointer entering this Components Image Space.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData) => IsInFocus = true;

    /// <summary>
    /// On Mouse Pointer exiting this Components Image Space.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData) => IsInFocus = false;

    public void Predict()
    {
        var mainTex = m_image.texture;
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

        client.Predict(input, output =>
        {
            output_tmp = new byte[output.Length];
            Array.Copy(output, 0, output_tmp, 0, output.Length);
            Debug.Log(output_tmp.Length);

        }, error =>
        {
            // TODO: when i am not lazy
        });
    }

    /// <summary>
    /// Exports the Sketch as a PNG.
    /// </summary>
    /// <param name="targetDirectory"></param>
    /// <param name="fileName"></param>
    public void ExportSketch(string targetDirectory, string fileName)
    {
       // tmp_text = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        //this.tmp_text.LoadImage(output_tmp);
        //this.tmp_text.Apply();
        //m_image.texture = this.tmp_text;

        var dt = DateTime.Now.ToString("yyMMdd_hhmmss");
        fileName += $"_{dt}";

        targetDirectory = Path.Combine(targetDirectory, "Pixel Drawings");

        /*
        var mainTex = m_image.texture;
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
        */
        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        var fp = Path.Combine(targetDirectory, fileName + ".png");

        if (File.Exists(fp))
            File.Delete(fp);

        File.WriteAllBytes(fp, output_tmp);

        System.Diagnostics.Process.Start(targetDirectory);
    }

    // Indices and weights of erosion brush precomputed for every node
    int[][] erosionBrushIndices;
    float[][] erosionBrushWeights;
    System.Random prng;

    int currentSeed;
    int currentErosionRadius;
    int currentMapSize;

    int seed = 10;
    public int erosionRadius = 3;
    public float initialWaterVolume = 1;
    public float initialSpeed = 1;
    public int maxDropletLifetime = 30;
    public float inertia = .05f; // At zero, water will instantly change direction to flow downhill. At 1, water will never change direction. 

    public float sedimentCapacityFactor = 4; // Multiplier for how much sediment a droplet can carry
    public float minSedimentCapacity = .01f; // Used to prevent carry capacity getting too close to zero on flatter terrain

    public float depositSpeed = .3f;

    public float erodeSpeed = .3f;

    public float evaporateSpeed = .01f;
    public float gravity = 4;


    // Initialization creates a System.Random object and precomputes indices and weights of erosion brush
    void Initialize(int mapSize, bool resetSeed)
    {
        if (resetSeed || prng == null || currentSeed != seed)
        {
            prng = new System.Random(seed);
            currentSeed = seed;
        }

        if (erosionBrushIndices == null || currentErosionRadius != erosionRadius || currentMapSize != mapSize)
        {
            InitializeBrushIndices(mapSize, erosionRadius);
            currentErosionRadius = erosionRadius;
            currentMapSize = mapSize;
        }
    }

    public void Erode(float[] map, int mapSize, int numIterations = 1, bool resetSeed = false)
    {
        Initialize(mapSize, resetSeed);

        for (int iteration = 0; iteration < numIterations; iteration++)
        {
            // Create water droplet at random point on map
            float posX = prng.Next(0, mapSize - 1);
            float posY = prng.Next(0, mapSize - 1);
            float dirX = 0;
            float dirY = 0;
            float speed = initialSpeed;
            float water = initialWaterVolume;
            float sediment = 0;

            for (int lifetime = 0; lifetime < maxDropletLifetime; lifetime++)
            {
                int nodeX = (int)posX;
                int nodeY = (int)posY;
                int dropletIndex = nodeY * mapSize + nodeX;
                // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
                float cellOffsetX = posX - nodeX;
                float cellOffsetY = posY - nodeY;

                // Calculate droplet's height and direction of flow with bilinear interpolation of surrounding heights
                HeightAndGradient heightAndGradient = CalculateHeightAndGradient(map, mapSize, posX, posY);

                // Update the droplet's direction and position (move position 1 unit regardless of speed)
                dirX = (dirX * inertia - heightAndGradient.gradientX * (1 - inertia));
                dirY = (dirY * inertia - heightAndGradient.gradientY * (1 - inertia));
                // Normalize direction
                float len = Mathf.Sqrt(dirX * dirX + dirY * dirY);
                if (len != 0)
                {
                    dirX /= len;
                    dirY /= len;
                }
                posX += dirX;
                posY += dirY;

                // Stop simulating droplet if it's not moving or has flowed over edge of map
                if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= mapSize - 1 || posY < 0 || posY >= mapSize - 1)
                {
                    break;
                }

                // Find the droplet's new height and calculate the deltaHeight
                float newHeight = CalculateHeightAndGradient(map, mapSize, posX, posY).height;
                float deltaHeight = newHeight - heightAndGradient.height;

                // Calculate the droplet's sediment capacity (higher when moving fast down a slope and contains lots of water)
                float sedimentCapacity = Mathf.Max(-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                // If carrying more sediment than capacity, or if flowing uphill:
                if (sediment > sedimentCapacity || deltaHeight > 0)
                {
                    // If moving uphill (deltaHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment
                    float amountToDeposit = (deltaHeight > 0) ? Mathf.Min(deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                    sediment -= amountToDeposit;

                    // Add the sediment to the four nodes of the current cell using bilinear interpolation
                    // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                    map[dropletIndex] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
                    map[dropletIndex + 1] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
                    map[dropletIndex + mapSize] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
                    map[dropletIndex + mapSize + 1] += amountToDeposit * cellOffsetX * cellOffsetY;

                }
                else
                {
                    // Erode a fraction of the droplet's current carry capacity.
                    // Clamp the erosion to the change in height so that it doesn't dig a hole in the terrain behind the droplet
                    float amountToErode = Mathf.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);

                    // Use erosion brush to erode from all nodes inside the droplet's erosion radius
                    for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices[dropletIndex].Length; brushPointIndex++)
                    {
                        int nodeIndex = erosionBrushIndices[dropletIndex][brushPointIndex];
                        float weighedErodeAmount = amountToErode * erosionBrushWeights[dropletIndex][brushPointIndex];
                        float deltaSediment = (map[nodeIndex] < weighedErodeAmount) ? map[nodeIndex] : weighedErodeAmount;
                        map[nodeIndex] -= deltaSediment;
                        sediment += deltaSediment;
                    }
                }

                // Update droplet's speed and water content
                speed = Mathf.Sqrt(speed * speed + deltaHeight * gravity);
                water *= (1 - evaporateSpeed);
            }
        }
    }

    HeightAndGradient CalculateHeightAndGradient(float[] nodes, int mapSize, float posX, float posY)
    {
        int coordX = (int)posX;
        int coordY = (int)posY;

        // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
        float x = posX - coordX;
        float y = posY - coordY;

        // Calculate heights of the four nodes of the droplet's cell
        int nodeIndexNW = coordY * mapSize + coordX;
        float heightNW = nodes[nodeIndexNW];
        float heightNE = nodes[nodeIndexNW + 1];
        float heightSW = nodes[nodeIndexNW + mapSize];
        float heightSE = nodes[nodeIndexNW + mapSize + 1];

        // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
        float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
        float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

        // Calculate height with bilinear interpolation of the heights of the nodes of the cell
        float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

        return new HeightAndGradient() { height = height, gradientX = gradientX, gradientY = gradientY };
    }

    void InitializeBrushIndices(int mapSize, int radius)
    {
        erosionBrushIndices = new int[mapSize * mapSize][];
        erosionBrushWeights = new float[mapSize * mapSize][];

        int[] xOffsets = new int[radius * radius * 4];
        int[] yOffsets = new int[radius * radius * 4];
        float[] weights = new float[radius * radius * 4];
        float weightSum = 0;
        int addIndex = 0;

        for (int i = 0; i < erosionBrushIndices.GetLength(0); i++)
        {
            int centreX = i % mapSize;
            int centreY = i / mapSize;

            if (centreY <= radius || centreY >= mapSize - radius || centreX <= radius + 1 || centreX >= mapSize - radius)
            {
                weightSum = 0;
                addIndex = 0;
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        float sqrDst = x * x + y * y;
                        if (sqrDst < radius * radius)
                        {
                            int coordX = centreX + x;
                            int coordY = centreY + y;

                            if (coordX >= 0 && coordX < mapSize && coordY >= 0 && coordY < mapSize)
                            {
                                float weight = 1 - Mathf.Sqrt(sqrDst) / radius;
                                weightSum += weight;
                                weights[addIndex] = weight;
                                xOffsets[addIndex] = x;
                                yOffsets[addIndex] = y;
                                addIndex++;
                            }
                        }
                    }
                }
            }

            int numEntries = addIndex;
            erosionBrushIndices[i] = new int[numEntries];
            erosionBrushWeights[i] = new float[numEntries];

            for (int j = 0; j < numEntries; j++)
            {
                erosionBrushIndices[i][j] = (yOffsets[j] + centreY) * mapSize + xOffsets[j] + centreX;
                erosionBrushWeights[i][j] = weights[j] / weightSum;
            }
        }
    }

    struct HeightAndGradient
    {
        public float height;
        public float gradientX;
        public float gradientY;
    }
}
