using System;
using System.Linq;
using UnityEngine;

public class ClearAreaNearRiverStep : GenerationStepBase
{               
    public override void Run()
    {
        data.roadGraph
            .GetNodes()
            .Where(IsInRiver)
            .ForEach(n => data.roadGraph.RemoveNode(n));        
        
        data.roadGraph
            .GetNodes()
            .Where(n => data.roadGraph.GetOutEdges(n).Count() <= 1)
            .ForEach(n => data.roadGraph.RemoveNode(n));
        
        data.cityPlan.RemoveCityBlockWhere(IsInRiver);
    }
    
    private bool IsInRiver(RoadNode node)
    {
        var minDistFromRiver = (options.riverWidth * options.blockSize) / 2f;
        Func<Vector3, bool> tooClose = (p) => Vector3.Distance(node.pos, p) < minDistFromRiver;
        var result = data.riverPath.Any(tooClose);
        
        return result;
    }
    
    private bool IsInRiver(CityBlockData cityBlock)
    {
        var minDistFromRiver = (1.41 * options.riverWidth * options.blockSize) / 2f;
        Func<Vector3, bool> tooClose = (p) => Vector3.Distance(cityBlock.center, p) < minDistFromRiver;
        var result = data.riverPath.Any(tooClose);
        
        return result;
    }
}
