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
    }
    
    private bool IsInRiver(RoadNode node)
    {
        var minDistFromRiver = (options.riverWidth * options.blockSize) / 2f;
        Func<Vector3, bool> tooClose = (p) => Vector3.Distance(node.pos, p) < minDistFromRiver;
        var result = data.riverGraph
            .GetNodes()
            .Select(n => n.pos)
            .Any(tooClose);
        
        return result;
    }
}
