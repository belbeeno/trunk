using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadGraph
{
    private Graph<Vector3> _graph = new Graph<Vector3>();
        
    public void AddIntersection(Vector3 pos)
    {       
        _graph.AddNode(pos);
    }
        
    public void RemoveIntersectionWhere(Func<Vector3, bool> check)
    {
        _graph.RemoveNodeWhere(check);
    }
    
    public Vector3 GetClosestIntersection(Vector3 point)
    {
        var bestNode = default(Vector3);
        var bestDistance = float.MaxValue;
        foreach (var node in _graph.nodes)
        {
            var distance = Vector3.Distance(node, point);
            if (distance < bestDistance)
            {
                bestNode = node;
                bestDistance = distance;
            }
        }
        
        return bestNode;
    }
    
    public void AddRoad(Vector3 from, Vector3 to)
    {
        if (_graph.ContainsNode(from) && _graph.ContainsNode(to))
        {
            _graph.AddEdge(from, to);
        }
    }
    
    public IEnumerable<RoadData> GetRoads()
    {
        var roads = new List<RoadData>();
        foreach (var edge in _graph.edges)
        {
            roads.Add(new RoadData(edge.from, edge.to));
        }
        return roads; 
    }
}