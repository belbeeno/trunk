using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerationData
{    
    public RoadGraph roadGraph { get; set; }
    public ICollection<CityBlockData> cityBlocks { get; set; }
    public ICollection<SidewalkData> sidewalks { get; set; }
    public ICollection<RoadData> roadMeshes { get; set; }
    public ICollection<BuildingPlotData> buildingPlots { get; set; }
    public RiverGraph riverGraph { get; set; }
        
    public GenerationResult ToGenerationResult()
    {
        var result = new GenerationResult
            {
                roadGraph = roadGraph,
                cityBlocks = cityBlocks.ToArray(),
                buildingPlots = buildingPlots.ToArray(),
                riverGraph = riverGraph,
                sidewalks = sidewalks.ToArray(),
                roadMeshes = roadMeshes.ToArray()
            };
            
        return result;
    }
}
