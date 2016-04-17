using UnityEngine;

public class BuildingPlotData
{
    public BuildingPlotData(Vector3[] corners, Mesh mesh, Material material)
    {
        this.corners = corners;
        this.mesh = mesh;
        this.material = material;
    }
    
    public readonly Vector3[] corners;
    public readonly Mesh mesh;
    public readonly Material material;
}
