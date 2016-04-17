using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateBuildingPlotsStep : GenerationStepBase
{
    private readonly Color _buildingColor = Color.grey;
    
    public override void Run()
    {
        var plots = data.cityBlocks
            .Where(b => !b.ContainsRiver())
            .Select(b => CreateBuildingPlot(b))
            .ToList();
        
        data.buildingPlots = plots;
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
        var numFloors = Random.Range(1, 6);
        var floorHeight = options.floorHeight * options.blockSize;
        
        Mesh generatedMesh = new Mesh();
        List<Vector3> points = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        for (int floor = 0; floor <= numFloors; floor++)
        {
            for (int corner = 0; corner < corners.Length; corner++)
            {
                points.Add(corners[corner] + Vector3.up * floorHeight * floor);
                colors.Add(_buildingColor);
                uvs.Add(new Vector2((float)corner, (float)floor));
                if (floor > 0)
                {
                    tris.Add((corner + 0) + (floor - 1) * corners.Length);
                    tris.Add((corner + 0) + (floor - 0) * corners.Length);
                    tris.Add((corner + 1) % corners.Length + (floor - 1) * corners.Length);

                    tris.Add((corner + 0) + (floor - 0) * corners.Length);
                    tris.Add((corner + 1) % corners.Length + (floor - 0) * corners.Length);
                    tris.Add((corner + 1) % corners.Length + (floor - 1) * corners.Length);
                }
            }
        }
        
        Vector3 averagePointsOnRoof = new Vector3();
        int lastIdx = points.Count;
        for (int i = 0; i < corners.Length; ++i)
        {
            averagePointsOnRoof += corners[i];
            tris.Add(lastIdx - (corners.Length - i));
            tris.Add(lastIdx);
            tris.Add(lastIdx - (corners.Length - (i + 1) % corners.Length));
        }
        averagePointsOnRoof *= 1f / corners.Length;
        averagePointsOnRoof.y = ((float)numFloors + 0.5f) * floorHeight;
        points.Add(averagePointsOnRoof);
        colors.Add(_buildingColor);
        uvs.Add(new Vector2(1f, Mathf.Max((float)numFloors - 1f, 0f)));
        
        generatedMesh.SetVertices(points);
        generatedMesh.SetColors(colors);
        generatedMesh.SetUVs(0, uvs);
        generatedMesh.SetIndices(tris.ToArray(), MeshTopology.Triangles, 0);

        return generatedMesh;
    }
}
