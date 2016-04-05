using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddBridgesStep : GenerationStepBase
{
    private IList<BridgeData> _bridges;
    
    public override GenerationData Run()
    {
        CreateAllBridges();
        PickBestBridges();
        
        return data;
    }
        
    private void CreateAllBridges()
    {
        _bridges = new List<BridgeData>();
        var currentPoint = data.riverPath.First();
        foreach (var point in data.riverPath.Skip(1))
        {
            var bridge = new BridgeData();
            var riverCenter = (point + currentPoint) / 2f;
            var perpendicular = new Vector3(-riverCenter.z, 0f, riverCenter.x);
            perpendicular.Normalize();
            perpendicular *= (options.riverWidth * options.blockSize) / 2f;
            
            bridge.intersection1 = data.roadGraph.GetClosestIntersection(riverCenter + perpendicular);
            bridge.intersection2 = data.roadGraph.GetClosestIntersection(riverCenter - perpendicular);
            bridge.center = (bridge.intersection1 + bridge.intersection2) / 2f;
            
            var bridgeDir = bridge.intersection2 - bridge.intersection1;
            bridge.angle = Mathf.Min(Vector3.Angle(perpendicular, bridgeDir), Vector3.Angle(-perpendicular, bridgeDir));
            
            _bridges.Add(bridge);
            currentPoint = point;
        }
    }
    
    private void PickBestBridges()
    {
        int numPicked = 0;
        while (numPicked < options.numBridges && _bridges.Count > 0)
        {
            var bestBridge = _bridges.OrderBy(b => b.angle).First();
            data.roadGraph.AddRoad(bestBridge.intersection1, bestBridge.intersection2);
            RemoveNearbyBridges(bestBridge);
            numPicked++;
        }
    }
    
    private void RemoveNearbyBridges(BridgeData bridge)
    {
        var bridgesCopy = new List<BridgeData>(_bridges);
        foreach (var other in bridgesCopy)
        {
            var shouldRemove = other.intersection1 == bridge.intersection1
                || other.intersection2 == bridge.intersection1
                || other.intersection1 == bridge.intersection2
                || other.intersection2 == bridge.intersection2
                || Vector3.Distance(bridge.center, other.center) < (2 * options.blockSize);
            
            if (shouldRemove)
            {
                _bridges.Remove(other);
            }
        }
    }
    
    private class BridgeData
    {        
        public Vector3 center;
        public Vector3 intersection1;
        public Vector3 intersection2;
        public float angle;
    }
}