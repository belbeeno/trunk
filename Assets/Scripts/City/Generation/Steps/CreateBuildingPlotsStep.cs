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
    
    public BuildingPlotData CreateBuildingPlot(CityBlockData city)
    {
        var corners = city.boundingRoads.Select(r => r.from.pos).ToArray();
        var numFloors = Random.Range(1, 6);
        var floorHeight = options.blockSize * options.floorHeight;
        var buildingPlot = new BuildingPlotData(corners, numFloors, floorHeight);
        
        return buildingPlot;
    }
}
