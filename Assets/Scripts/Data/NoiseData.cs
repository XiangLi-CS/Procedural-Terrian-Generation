using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : AutoUpdateData
{
    public Noise.NormalizeMode normalizeMode;

    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    protected override void _OnValidate()
    {

        if (octaves < 0)
        {
            octaves = 0;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        base._OnValidate();

    }

}
