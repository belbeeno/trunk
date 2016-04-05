using System;
using UnityEngine;

[Serializable]
public class GenerationOptions
{
    [Range(1, 32)] public int blocksWidth;
    [Range(1, 32)] public int blocksHeight;
    
    [Range(4f, 8f)] public float blockSize;
    public float cityWidth { get { return blocksWidth * blockSize; } }
    public float cityHeight { get { return blocksHeight * blockSize; } }
    
    [Range(0f, 1f)] public float roadWidth;
    [Range(0f, 8f)] public float floorHeight;
    
    [Range(0.5f, 2f)] public float riverWidth;
    [Range(1, 64)] public int numRiverSegments;
}
