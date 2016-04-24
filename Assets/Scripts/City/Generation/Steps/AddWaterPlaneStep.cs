using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddWaterPlaneStep : GenerationStepBase
{               
    private Color _waterColor = new Color(0f, 0f, 0.5f, 1f);
    
    public override void Run()
    {
        var corners = new []
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(options.cityHeight, 0f, 0f),
                new Vector3(options.cityHeight, 0f, options.cityWidth),
                new Vector3(0f, 0f, options.cityWidth)
            }.Outset(10 * options.blockSize).ToArray();
        
        var mesh = GetMesh(corners);
        var material = MaterialsStore.instance.water;
        data.water = new WaterData(corners, mesh, material);
    }
    
    private Mesh GetMesh(Vector3[] corners)
    {        
        var waterLevel = 0.05f * options.blockSize;
        
        Mesh generatedMesh = new Mesh();
        List<Vector3> points = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        // Bottom row
        for (int corner = 0; corner < corners.Length; corner++)
        {
            points.Add(corners[corner] + Vector3.down * waterLevel);
            colors.Add(_waterColor);
            uvs.Add(GetUV(corners[corner]));
        }
        
        // Solid top
        int target = points.Count - corners.Length;
        for (int i = target + 1; i < points.Count - 1; i++)
        {
            tris.Add(target);
            tris.Add(i + 1);
            tris.Add(i);
        }
        
        generatedMesh.SetVertices(points);
        generatedMesh.SetColors(colors);
        generatedMesh.SetUVs(0, uvs);
        generatedMesh.SetIndices(tris.ToArray(), MeshTopology.Triangles, 0);

        return generatedMesh;
    }
    
    private Vector2 GetUV(Vector3 point)
    {
        var textureScale = 10f / options.blockSize;
        var u = point.x * textureScale;
        var v = point.z * textureScale; 
        return new Vector2(u, v);
    }
}
