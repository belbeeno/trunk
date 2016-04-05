using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine;

public class ClearAreaNearRiverStep : GenerationStepBase
{               
    public override GenerationData Run()
    {
        data.roadGraph.RemoveIntersectionWhere(IsInRiver);
        data.cityPlan.RemoveCityBlockWhere(IsInRiver);
        
        return data;
    }
    
    private bool IsInRiver(Vector3 point)
    {
        var minDistFromRiver = options.riverWidth * options.blockSize;
        Func<Vector3, bool> tooClose = (p) => Vector3.Distance(point, p) < minDistFromRiver;
        var result = data.riverPath.Any(tooClose);
        
        return result;
    }
    
    private bool IsInRiver(CityBlockData cityBlock)
    {
        var minDistFromRiver = 1.5 * options.riverWidth * options.blockSize;
        Func<Vector3, bool> tooClose = (p) => Vector3.Distance(cityBlock.center, p) < minDistFromRiver;
        var result = data.riverPath.Any(tooClose);
        
        return result;
    }
}
