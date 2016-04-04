using System;
using UnityEngine;

[Serializable]
public class GenerationOptions
{
    [Range(0, 32)] public int width;
    [Range(0, 32)] public int height;
    
    [Range(0f, 8f)] public float floorHeight;
    [Range(4f, 8f)] public float blockSize;
    [Range(1f, 4f)] public float roadWidth;
    
    [Range(0f, 8f)] public float riverWidth;
    [Range(0, 64)] public int numRiverSegments;
}
