using System;
using System.Linq;
using UnityEngine;

public class ClearAreaNearRiverStep : GenerationStepBase
{               
    public override void Run()
    {
        data.roadGraph.RemoveIntersectionWhere(IsInRiver);
        data.roadGraph.RemoveDeadEnds();
        data.cityPlan.RemoveCityBlockWhere(IsInRiver);
    }
    
    private bool IsInRiver(Node<RoadNodeData> node)
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
