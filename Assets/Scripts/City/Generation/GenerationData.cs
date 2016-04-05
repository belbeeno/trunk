using System.Collections.Generic;
using UnityEngine;

public class GenerationData
{
    public GenerationData(GenerationOptions options)
    {
        roadGraph = new RoadGraph();
        cityPlan = new CityPlan(options);
    }
    
    public RoadGraph roadGraph { get; set; }
    public CityPlan cityPlan { get; set; }
    public IList<Vector3> riverPath { get; set; }
    
    public GenerationResult ToGenerationResult()
    {
        var result = new GenerationResult
            {
                roads = roadGraph.GetRoads(),
                cityBlocks = cityPlan.GetCityBlocks(),
                river = new RiverData(riverPath)
            };
            
        return result;
    }
}
