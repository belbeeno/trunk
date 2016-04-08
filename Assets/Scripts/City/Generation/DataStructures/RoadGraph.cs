using System;
using System.Linq;
using UnityEngine;

public class RoadGraph
{
    private Graph<RoadNodeData, RoadEdgeData> _graph = new Graph<RoadNodeData, RoadEdgeData>();
        
    public void AddIntersectionNode(Vector3 pos)
    {       
        _graph.AddNode(new Node<RoadNodeData>(pos, new RoadNodeData()));
    }
        
    public void RemoveIntersectionWhere(Func<Node<RoadNodeData>, bool> check)
    {
        foreach (var node in _graph.GetNodes().Where(check))
        {
            _graph.RemoveNode(node);
        }
    }
    
    public Node<RoadNodeData>[] GetIntersections()
    {
        return _graph.GetNodes();
    }

    public Node<RoadNodeData> GetClosestIntersection(Vector3 point)
    {
        var bestNode = default(Node<RoadNodeData>);
        var bestDistance = float.MaxValue;
        foreach (var node in _graph.GetNodes())
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
    
    public void AddRoad(Vector3 from, Vector3 to, bool isBridge)
    {
        var fromNode = new Node<RoadNodeData>(from, new RoadNodeData());
        var toNode = new Node<RoadNodeData>(to, new RoadNodeData());        
        if (_graph.ContainsNode(fromNode) && _graph.ContainsNode(toNode))
        {
            var edge = new Edge<RoadNodeData, RoadEdgeData>(fromNode, toNode, new RoadEdgeData(isBridge));
            _graph.AddUndirectedEdge(edge);
        }
    }
    
    public RoadData[] GetRoads()
    {
        var roads = _graph.GetEdges()
            .Select(e => new RoadData(e.from.pos, e.to.pos))
            .ToArray();
            
        return roads; 
    }
    
    public void RemoveDeadEnds()
    {
        foreach (var node in _graph.GetNodes())
        {
            var numNeighbours = _graph.GetOutEdges(node).Count();
            if (numNeighbours <= 1)
            {
                _graph.RemoveNode(node);
            }
        }
    }
}