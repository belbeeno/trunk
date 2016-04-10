using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddBridgesStep : GenerationStepBase
{
    private IList<BridgeData> _bridges;
    
    public override void Run()
    {
        CreateAllBridges();
        PickBestBridges();
    }
        
    private void CreateAllBridges()
    {
        _bridges = new List<BridgeData>();
                
        foreach (var edge in data.riverGraph.GetEdges())
        {
            var bridge = new BridgeData();
            
            var perpendicular = new Vector3(-edge.direction.z, 0f, edge.direction.x).normalized;
            perpendicular *= (options.riverWidth * options.blockSize) / 2f;
            
            bridge.intersection1 = GetClosestIntersection(edge.center + perpendicular).pos;
            bridge.intersection2 = GetClosestIntersection(edge.center - perpendicular).pos;
            bridge.center = (bridge.intersection1 + bridge.intersection2) / 2f;
            
            var bridgeDir = bridge.intersection2 - bridge.intersection1;
            bridge.angle = Mathf.Min(Vector3.Angle(perpendicular, bridgeDir), Vector3.Angle(-perpendicular, bridgeDir));
            
            _bridges.Add(bridge);
        }
    }
    
    private void PickBestBridges()
    {
        int numPicked = 0;
        while (numPicked < options.numBridges && _bridges.Count > 0)
        {
            var bestBridge = _bridges.OrderBy(b => b.angle).First();
            var edgeData = new RoadEdgeData(isBridge: true);
            data.roadGraph.AddUndirectedEdge(bestBridge.intersection1, bestBridge.intersection2, edgeData);
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
    
    public RoadNode GetClosestIntersection(Vector3 point)
    {
        var bestNode = default(RoadNode);
        var bestDistance = float.MaxValue;
        foreach (var node in data.roadGraph.GetNodes())
        {
            var distance = Vector3.Distance(node.pos, point);
            if (distance < bestDistance)
            {
                bestNode = node;
                bestDistance = distance;
            }
        }
        
        return bestNode;
    }
    
    private class BridgeData
    {        
        public Vector3 center;
        public Vector3 intersection1;
        public Vector3 intersection2;
        public float angle;
    }
}
