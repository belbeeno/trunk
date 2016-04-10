using System.Collections.Generic;
using System.Linq;

public class GenerationData
{    
    public RoadGraph roadGraph { get; set; }
    public ICollection<CityBlockData> cityBlocks { get; set; }
    public ICollection<BuildingPlotData> buildingPlots { get; set; }
    public RiverGraph riverGraph { get; set; }
    
    public GenerationResult ToGenerationResult()
    {
        var result = new GenerationResult
            {
                roadGraph = roadGraph,
                cityBlocks = cityBlocks.ToArray(),
                buildingPlots = buildingPlots.ToArray(),
                riverGraph = riverGraph
            };
            
        return result;
    }
}
