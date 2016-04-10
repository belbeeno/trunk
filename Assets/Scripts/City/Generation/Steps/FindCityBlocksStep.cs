using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindCityBlocksStep : GenerationStepBase
{    
    public override void Run()
    {
        data.cityBlocks = new List<CityBlockData>();
        
        var graphCopy = new RoadGraph(data.roadGraph);
        while (graphCopy.GetEdges().Any())
        {
            var cityBlock = GetPlot(graphCopy);
            if (cityBlock.boundingRoads.Count() < (options.blocksHeight + options.blocksWidth))
            {
                data.cityBlocks.Add(cityBlock);
            }
        }
    }
    
    public CityBlockData GetPlot(RoadGraph graph)
    {
        var currentEdge = graph.GetEdges().First();
        graph.RemoveDirectedEdge(currentEdge);
        var startPoint = currentEdge.from;
        
        var boundingRoads = new List<RoadEdge> { currentEdge }; 
        while (currentEdge.to != startPoint)
        {
            currentEdge = GetCCWAdjacentEdge(graph, currentEdge);
            graph.RemoveDirectedEdge(currentEdge);
            boundingRoads.Add(currentEdge);
        }
        
        var cityBlock = new CityBlockData(boundingRoads.ToArray());
        
        return cityBlock;
    }
    
    public RoadEdge GetCCWAdjacentEdge(RoadGraph graph, RoadEdge edge)
    {
        var ccwEdge = graph.GetOutEdges(edge.to)
            .OrderBy(other => GetRotation(edge.direction, other.direction))
            .First();
            
        return ccwEdge;
    }
    
    public float GetRotation(Vector3 from, Vector3 to)
    {
        var sign = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(from, to)));
        var angle = sign * Vector3.Angle(from, to);

        return angle;
    }
}
