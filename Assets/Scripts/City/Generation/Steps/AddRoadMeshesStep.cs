using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddRoadMeshesStep : GenerationStepBase
{
    private readonly Color _roadColor = Color.black;
    
    public override void Run()
    {
        var roadMeshes = data.roadGraph.GetEdges()
            .Select(b => CreateRoadMesh(b))
            .ToList();
        
        data.roadMeshes = roadMeshes;
    }
    
    private RoadData CreateRoadMesh(RoadEdge roadEdge)
    {
        var corners = GetCorners(roadEdge.from.pos, roadEdge.to.pos);
        var mesh = GetMesh(corners.ToArray());
        var material = MaterialsStore.instance.roads;
        var roadData = new RoadData(corners.ToArray(), mesh, material);
        
        return roadData;
    }
    
    private Vector3[] GetCorners(Vector3 from, Vector3 to)
    {
        var roadWidth = options.roadWidth * options.blockSize / 2f;
        var perpendicular = Line.CreateThroughPoints(from, to).Perpendicular() * roadWidth;
        var newFrom = from + (from - to).normalized * roadWidth;
        var newTo = to + (to - from).normalized * roadWidth;
 
        var corners = new List<Vector3>();
        corners.Add(newFrom);
        corners.Add(newFrom + perpendicular);
        corners.Add(newTo + perpendicular);
        corners.Add(newTo);
        return corners.ToArray();
    }
    
    private Mesh GetMesh(Vector3[] corners)
    {
        var roadHeight = options.blockSize;
        
        Mesh generatedMesh = new Mesh();
        List<Vector3> points = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        // Bottom row
        for (int corner = 0; corner < corners.Length; corner++)
        {
            points.Add(corners[corner] + Vector3.down * roadHeight);
            colors.Add(_roadColor);
            uvs.Add(GetUV(corners[corner]));
        }
        
        // Top row
        for (int corner = 0; corner < corners.Length; corner++)
        {
            points.Add( corners[corner]);
            colors.Add(_roadColor);
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
