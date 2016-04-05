using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddBridgesStep : GenerationStepBase
{
    private IList<Pair<Vector3, Vector3>> _segments;
    
    public override GenerationData Run()
    {
        PickBridgePoints();
        AddBridges();
        
        return data;
    }
        
    private void PickBridgePoints()
    {
        var offset = ((float)data.riverPath.Count) / (options.numBridges + 1);
        _segments = new List<Pair<Vector3, Vector3>>();
        
        for (var i = 1; i <= options.numBridges; i++)
        {
            var index = Convert.ToInt32(i * offset);
            _segments.Add(Pair<Vector3, Vector3>.Create(data.riverPath[index], data.riverPath[index + 1]));
        }
    }
    
    private void AddBridges()
    {
        foreach (var segment in _segments)
        {
            var segmentAverage = (segment.first + segment.second) / 2;
            
            var perpendicular = new Vector3(-segmentAverage.z, 0f, segmentAverage.x);
            perpendicular.Normalize();
            perpendicular *= (options.riverWidth * options.blockSize);
            
            var leftPoint = segmentAverage + perpendicular;
            var rightPoint = segmentAverage - perpendicular;

            var bridgeAnchor1 = data.roadGraph.GetClosestIntersection(leftPoint);
            var bridgeAnchor2 = data.roadGraph.GetClosestIntersection(rightPoint);
            var bridgeDir = bridgeAnchor1 - bridgeAnchor2;
            
            var isValid = Vector3.Distance(leftPoint, bridgeAnchor1) < Vector3.Distance(rightPoint, bridgeAnchor1)
                && Vector3.Distance(rightPoint, bridgeAnchor2) < Vector3.Distance(leftPoint, bridgeAnchor2)
                && Vector3.Angle(bridgeDir, perpendicular) < 40;
            
            if (isValid)
            {
                data.roadGraph.AddRoad(bridgeAnchor1, bridgeAnchor2);
            }
        }
    }
}