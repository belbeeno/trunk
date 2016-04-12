using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateBuildingPlotsStep : GenerationStepBase
{               
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
        var corners = GetInsetCorners(city.boundingRoads);
        var numFloors = Random.Range(1, 6);
        var floorHeight = options.blockSize * options.floorHeight;
        var buildingPlot = new BuildingPlotData(corners, numFloors, floorHeight);
        
        return buildingPlot;
    }
    
    private Vector3[] GetInsetCorners(RoadEdge[] edges)
    {
        // Inset boundaries
        var insetAmount = (options.roadWidth / 2f) * options.blockSize;
        var lines = edges.Select(e => Line.CreateThroughPoints(e.from.pos, e.to.pos));
        var insetLines = lines.Select(l => l.Offset(l.Perpendicular() * insetAmount));
        
        // Find intersections
        var corners = new List<Vector3>();
        var prevLine = insetLines.Last();
        foreach (var line in insetLines)
        {
            corners.Add(Line.Intersection(prevLine, line));
            prevLine = line;    
        }

        return corners.ToArray();
    }
}
