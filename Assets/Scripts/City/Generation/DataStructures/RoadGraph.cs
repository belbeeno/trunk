using System;
using System.Linq;
using UnityEngine;

public class RoadGraph
{
    public Graph<RoadNodeData, RoadEdgeData> graph = new Graph<RoadNodeData, RoadEdgeData>();
        
    public void AddIntersectionNode(Vector3 pos)
    {       
        graph.AddNode(pos, new RoadNodeData());
    }
        
    public void RemoveIntersectionWhere(Func<Node<RoadNodeData>, bool> check)
    {
        foreach (var node in graph.GetNodes().Where(check))
        {
            graph.RemoveNode(node);
        }
    }
    
    public Node<RoadNodeData>[] GetIntersections()
    {
        return graph.GetNodes();
    }

    public Node<RoadNodeData> GetClosestIntersection(Vector3 point)
    {
        var bestNode = default(Node<RoadNodeData>);
        var bestDistance = float.MaxValue;
        foreach (var node in graph.GetNodes())
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
        if (graph.ContainsNode(from) && graph.ContainsNode(to))
        {
            graph.AddUndirectedEdge(from, to, new RoadEdgeData(isBridge));
        }
    }
    
    public RoadData[] GetRoads()
    {
        var roads = graph.GetEdges()
            .Select(e => new RoadData(e.from.pos, e.to.pos))
            .ToArray();
            
        return roads; 
    }
    
    public void RemoveDeadEnds()
    {
        foreach (var node in graph.GetNodes())
        {
            var numNeighbours = graph.GetOutEdges(node).Count();
            if (numNeighbours <= 1)
            {
                graph.RemoveNode(node);
            }
        }
    }
}