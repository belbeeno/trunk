using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AddSidewalksStep : GenerationStepBase
{
    private readonly Color _sidewalkColor = Color.white;
    
    public override void Run()
    {
        var sidewalks = data.cityBlocks
            .Where(c => !c.ContainsRiver())
            .Select(b => CreateSidewalk(b))
            .ToList();
        
        data.sidewalks = sidewalks;
    }
    
    private SidewalkData CreateSidewalk(CityBlockData cityBlock)
    {
        var insetAmount = (options.roadWidth / 2f) * options.blockSize;
        var corners = cityBlock.boundingRoads.Select(p => p.from.pos).Inset(insetAmount);
        var mesh = GetMesh(corners);
        var material = GetMaterial();
        var sidewalk = new SidewalkData(corners, mesh, material);
        
        return sidewalk;
    }
    
    private Material GetMaterial()
    {
        string[] guids = AssetDatabase.FindAssets("BuildingMat");
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        
        return material;
    }
    
    private Mesh GetMesh(Vector3[] corners)
    {
        var sidewalkHeight = 0.001f * options.blockSize;
        
        Mesh generatedMesh = new Mesh();
        List<Vector3> points = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        // Bottom row
        for (int corner = 0; corner < corners.Length; corner++)
        {
            points.Add(corners[corner]);
            colors.Add(_sidewalkColor);
            uvs.Add(GetUV(corners[corner]));
        }
        
        // Top row
        for (int corner = 0; corner < corners.Length; corner++)
        {
            points.Add( corners[corner] + Vector3.up * sidewalkHeight);
            colors.Add(_sidewalkColor);
            uvs.Add(GetUV( corners[corner]));
            
            tris.Add(corner);
            tris.Add(corner + corners.Length);
            tris.Add((corner + 1) % corners.Length);

            tris.Add(corner + corners.Length);
            tris.Add((corner + 1) % corners.Length + corners.Length);
            tris.Add((corner + 1) % corners.Length);
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
