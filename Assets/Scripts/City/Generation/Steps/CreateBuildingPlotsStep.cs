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
            .Where(b => !b.IsCityFeature())
            .SelectMany(b => CreateBuildingPlots(b))
            .ToList();
        
        data.buildingPlots = plots;
    }
    
    private BuildingPlotData[] CreateBuildingPlots(CityBlockData city)
    {
        var insetAmount = ((options.roadWidth / 2f) + options.sidewalkWidth) * options.blockSize;
        var insetCorners = city.boundingRoads.Select(p => p.from.pos).Inset(insetAmount);
        var plots = Subdivide(insetCorners)
            .Select(p => p.Inset(options.blockSize * options.alleyWidth / 2f).ToArray())
            .Select(p => CreateBuildingPlot(p))
            .ToArray();
            
        return plots;
    }
    
    private Vector3[][] Subdivide(Vector3[] corners)
    {
        var points = new List<Vector3[]>();
        for (var x = 0; x < 3; x++)
        for (var y = 0; y < 3; y++)
        {
            if (x == 1 && y == 1)
            {
                continue;
            }
            
            points.Add(new [] {
                DoubleLerp(corners, x/3f, y/3f),
                DoubleLerp(corners, (x+1)/3f, y/3f),
                DoubleLerp(corners, (x+1)/3f, (y+1)/3f),
                DoubleLerp(corners, x/3f, (y+1)/3f)
            });
        }
        
        return points.ToArray();
    }
    
    private Vector3 DoubleLerp(Vector3[] corners, float horizT, float vertT)
    {
        var p1 = Vector3.Lerp(corners[0], corners[1], horizT);
        var p2 = Vector3.Lerp(corners[3], corners[2], horizT);
        var p3 = Vector3.Lerp(p1, p2, vertT);
        return p3;
    }
    
    private BuildingPlotData CreateBuildingPlot(Vector3[] corners)
    {
        var mesh = GetMesh(corners);
        var material = MaterialsStore.instance.sidewalks;
        var buildingPlot = new BuildingPlotData(corners, mesh, material);
        
        return buildingPlot;
    }
    
    private Mesh GetMesh(Vector3[] corners)
    {
        var numFloors = Random.Range(1, 4);
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
