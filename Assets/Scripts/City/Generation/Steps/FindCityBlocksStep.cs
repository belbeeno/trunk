using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindCityBlocksStep : GenerationStepBase
{    
    public override void Run()
    {
        data.cityPlan = new CityPlan();
        
        var graphCopy = new RoadGraph(data.roadGraph);
        while (graphCopy.GetEdges().Any())
        {
            var cityBlock = GetPlot(graphCopy);
            if (cityBlock.corners.Count() < (options.blocksHeight + options.blocksWidth))
            {
                data.cityPlan.AddCityBlock(cityBlock);
            }
        }
    }
    
    public CityBlockData GetPlot(RoadGraph graph)
    {
        var currentEdge = graph.GetEdges().First();
        graph.RemoveDirectedEdge(currentEdge);
        var startPoint = currentEdge.from;
        var isWaterPlot = false;
        
        var plotPoints = new List<Vector3> { startPoint.pos }; 
        while (currentEdge.to != startPoint)
        {
            currentEdge = GetCCWAdjacentEdge(graph, currentEdge);
            graph.RemoveDirectedEdge(currentEdge);
            plotPoints.Add(currentEdge.from.pos);
            
            if (currentEdge.data.isBridge) 
            {
                isWaterPlot = true;
            }
        }
        
        var cityBlock = GetCityBlock(plotPoints, isWaterPlot);
        
        return cityBlock;
    }
    
    private CityBlockData GetCityBlock(IList<Vector3> corners, bool isWaterPlot)
    {
        var numFloors = Random.Range(1, 6);
        var scaledHeight = options.floorHeight * options.blockSize;
        var cityBlock = new CityBlockData(corners.ToArray(), numFloors, scaledHeight, isWaterPlot);
        
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
