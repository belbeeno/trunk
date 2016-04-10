using UnityEngine;

public class CityBlockData
{
    public CityBlockData(Vector3[] corners, int numFloors, float floorHeight, bool isWaterPlot)
    {
        this.center = corners.Average();
        this.corners = corners;
        this.numFloors = numFloors;
        this.floorHeight = floorHeight;
        this.isWaterPlot = isWaterPlot;
    }
    
    public readonly Vector3 center;
    public readonly Vector3[] corners;
    public readonly int numFloors;
    public readonly float floorHeight;
    public readonly bool isWaterPlot;
}