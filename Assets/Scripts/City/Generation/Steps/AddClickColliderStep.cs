using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddClickColliderStep : GenerationStepBase
{               
    public override void Run()
    {
        var corners = new []
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(options.cityHeight, 0f, 0f),
                new Vector3(options.cityHeight, 0f, options.cityWidth),
                new Vector3(0f, 0f, options.cityWidth)
            };
        
        data.clickColliderMesh = GetMesh(corners);
    }
    
    private Mesh GetMesh(Vector3[] corners)
    {        
        Mesh generatedMesh = new Mesh();
        List<Vector3> points = new List<Vector3>();
        List<int> tris = new List<int>();

        for (int corner = 0; corner < corners.Length; corner++)
        {
            points.Add(corners[corner]);
        }
        
        int target = points.Count - corners.Length;
        for (int i = target + 1; i < points.Count - 1; i++)
        {
            tris.Add(target);
            tris.Add(i + 1);
            tris.Add(i);
        }
        
        generatedMesh.SetVertices(points);
        generatedMesh.SetIndices(tris.ToArray(), MeshTopology.Triangles, 0);

        return generatedMesh;
    }
}
