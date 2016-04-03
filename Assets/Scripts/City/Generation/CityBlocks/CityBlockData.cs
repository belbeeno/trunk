using UnityEngine;

public class CityBlockData
{
    public CityBlockData(Vector3[] corners, int numFloors, float floorHeight)
    {
        this.corners = corners;
        this.numFloors = numFloors;
        this.floorHeight = floorHeight;
    }
    
    public Vector3[] corners;
    public int numFloors;
    public float floorHeight;
}