using UnityEngine;

public class CityBlockData
{
    public CityBlockData(Vector3 center, Vector3[] corners, int numFloors, float floorHeight)
    {
        this.center = center;
        this.corners = corners;
        this.numFloors = numFloors;
        this.floorHeight = floorHeight;
    }
    
    public Vector3 center;
    public Vector3[] corners;
    public int numFloors;
    public float floorHeight;
}