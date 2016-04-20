using UnityEngine;

public class ParkData
{
    public ParkData(Vector3[] corners, Mesh mesh, Material material, GameObject prefab)
    {
        this.corners = corners;
        this.mesh = mesh;
        this.material = material;
        this.prefab = prefab;
    }
    
    public readonly Vector3[] corners;
    public readonly Mesh mesh;
    public readonly Material material;
    public readonly GameObject prefab;
}
