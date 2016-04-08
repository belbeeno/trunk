using System;
using UnityEngine;

[Serializable]
public class GenerationOptions
{
    [Header("City Size")]
    [Range(1, 32)] public int blocksWidth;
    [Range(1, 32)] public int blocksHeight;
    [Range(10f, 200f)] public float blockSize;
    public float cityWidth { get { return blocksWidth * blockSize; } }
    public float cityHeight { get { return blocksHeight * blockSize; } }
    
    [Header("Building Size")]
    [Range(0f, 1f)] public float roadWidth;
    [Range(0f, 2f)] public float floorHeight;
    
    [Header("River")]
    [Range(0.5f, 2f)] public float riverWidth;
    [Range(1, 64)] public int numRiverSegments;
    [Range(0, 5)] public int numBridges;
}
