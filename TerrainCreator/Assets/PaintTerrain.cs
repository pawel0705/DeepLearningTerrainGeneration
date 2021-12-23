using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTerrain : MonoBehaviour
{
    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public int startingHeight;
        public int overlap;
    }

    public SplatHeights[] splatHeights;

    void normalize(float[] v)
    {
        float total = 0;
        for(int i =0; i < v.Length; i++)
        {
            total += v[i];
        }

        for(int i = 0; i < v.Length; i++)
        {
            v[i] /= total;
        }
    }

    public float map(float value, float sMin, float sMax, float mMin, float mMax)
    {
        return (value - sMin) * (mMax - mMin) / (sMax - sMin) + mMin;
    }

    // Start is called before the first frame update
    void Start()
    {
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        float[,,] splatmapData = new float[terrainData.alphamapWidth,
            terrainData.alphamapHeight,
            terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for(int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(y, x);
                float[] splat = new float[splatHeights.Length];

                for(int i = 0; i < splatHeights.Length; i++)
                {
                    float thisNoise = map(Mathf.PerlinNoise(x * 0.05f, y * 0.05f),0,1, 0.5f, 1);

                    float thisHeightStart = splatHeights[i].startingHeight * thisNoise
                        - splatHeights[i].overlap * thisNoise;
                    float nextHeightStart = 0;

                    if(i != splatHeights.Length - 1)
                    {
                        nextHeightStart = splatHeights[i + 1].startingHeight * thisNoise
                            + splatHeights[i + 1].overlap * thisNoise;
                    }

                    if (i==splatHeights.Length - 1 && terrainHeight >= thisHeightStart)
                    {
                        splat[i] = 1;
                    }
                    else if(terrainHeight >= thisHeightStart && terrainHeight <= nextHeightStart)
                    {
                        splat[i] = 1;
                    }
                }

                normalize(splat);

                for(int j = 0; j < splatHeights.Length; j++)
                {
                    splatmapData[x, y, j] = splat[j];
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
