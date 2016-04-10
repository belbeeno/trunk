using UnityEngine;

public class BuildingPlotData
{
    public BuildingPlotData(Vector3[] corners, int numFloors, float floorHeight)
    {
        this.center = corners.Average();
        this.corners = corners;
        this.numFloors = numFloors;
        this.floorHeight = floorHeight;
    }
    
    public readonly Vector3 center;
    public readonly Vector3[] corners;
    public readonly int numFloors;
    public readonly float floorHeight;
}