using System.Collections.Generic;
using UnityEngine;

public class GenerationData
{    
    public RoadGraph roadGraph { get; set; }
    public CityPlan cityPlan { get; set; }
    public RiverGraph riverGraph { get; set; }
    
    public GenerationResult ToGenerationResult()
    {
        var result = new GenerationResult
            {
                roadGraph = roadGraph,
                cityBlocks = cityPlan.GetCityBlocks(),
                riverGraph = riverGraph
            };
            
        return result;
    }
}
