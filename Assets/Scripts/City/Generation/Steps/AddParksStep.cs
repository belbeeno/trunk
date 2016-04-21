using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddParksStep : GenerationStepBase
{    
    private Color _parkColor = new Color(0f, 0.4f, 0f, 1f);
      
    public override void Run()
    {
        data.smallParks = data.cityBlocks
            .Where(b => b.isSmallPark)
            .Select(b => AddPark(b, PrefabStore.instance.smallPark))
            .ToArray();
        
        data.largeParks = data.cityBlocks
            .Where(b => b.isLargePark)
            .Select(b => AddPark(b, PrefabStore.instance.largePark))
            .ToArray();
    }
    
    private ParkData AddPark(CityBlockData block, GameObject prefab)
    {        
        var insetAmount = (options.roadWidth / 2f) * options.blockSize;
        var corners = block.boundingRoads.Select(p => p.from.pos).Inset(insetAmount);
        var mesh = GetMesh(corners);
        var material = MaterialsStore.instance.grass;
        var park = new ParkData(corners, mesh, material, prefab);
        return park;
    }
    
    private BuildingPlotData CreateBuildingPlot(CityBlockData city)
    {
        var insetAmount = ((options.roadWidth / 2f) + options.sidewalkWidth) * options.blockSize;
        var corners = city.boundingRoads.Select(p => p.from.pos).Inset(insetAmount);
        var mesh = GetMesh(corners);
        var material = MaterialsStore.instance.buildings;
        var buildingPlot = new BuildingPlotData(corners, mesh, material);
        
        return buildingPlot;
    }
    
    private Mesh GetMesh(Vector3[] corners)
    {        
        Mesh generatedMesh = new Mesh();
        List<Vector3> points = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        // Bottom row
        for (int corner = 0; corner < corners.Length; corner++)
        {
            points.Add(corners[corner]);
            colors.Add(_parkColor);
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
