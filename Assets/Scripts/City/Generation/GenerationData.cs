using System.Collections.Generic;
using UnityEngine;

public class GenerationData
{    
    public RoadGraph roadGraph { get; set; }
    public CityPlan cityPlan { get; set; }
    public IList<Vector3> riverPath { get; set; }
    
    public GenerationResult ToGenerationResult()
    {
        var result = new GenerationResult
            {
                roadGraph = roadGraph.graph,
                roads = roadGraph.GetRoads(),
                cityBlocks = cityPlan.GetCityBlocks(),
                river = new RiverData(riverPath)
            };
            
        return result;
    }
}
