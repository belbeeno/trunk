using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddParksStep : GenerationStepBase
{             
    private IList<ParkData> _parks;
    
    private Color _parkColor = new Color(0f, 0.4f, 0f, 1f);
      
    public override void Run()
    {
        _parks = new List<ParkData>();
        while (_parks.Count() < options.numParks)
        {
            AddPark();
        }
        
        data.parks = _parks.ToArray();
    }
    
    private void AddPark()
    {
        var block = data.cityBlocks.Where(c => !c.ContainsRiver()).RandomMember();
        var insetAmount = (options.roadWidth / 2f) * options.blockSize;
        var corners = block.boundingRoads.Select(p => p.from.pos).Inset(insetAmount);
        var mesh = GetMesh(corners);
        var material = MaterialsStore.instance.basic;
        var park = new ParkData(corners, mesh, material);
        
        var isOkay = _parks
            .Select(p => p.corners.Average())
            .Select(c => Vector3.Distance(c, park.corners.Average()))
            .All(d => d > options.parksDist * options.blockSize);

        if (isOkay)
        {
            block.isPark = true;
            _parks.Add(park);
        }
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
