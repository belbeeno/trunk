using System;
using System.Collections.Generic;
using UnityEngine;

public class GridRoadGraph : IRoadGraph
{
    private Graph<Point2D> _graph = new Graph<Point2D>();
    
    private float _gridSize;
    
    public GridRoadGraph(float gridSize)
    {
        _gridSize = gridSize;
    }
    
    public IEnumerable<RoadData> roads
    {
        get
        {
            var roads = new List<RoadData>();
            foreach (var edge in _graph.edges)
            {
                roads.Add(new RoadData(ToVector3(edge.from), ToVector3(edge.to)));
            }
            return roads; 
        }
    }
    
    public void AddIntersection(int x, int y)
    {
        var node = new Point2D(x, y);
        if (_graph.ContainsNode(node)) 
        {
            return;    
        }
        
        _graph.AddNode(node);
        TryAddRoad(node, new Point2D(x, y - 1));
        TryAddRoad(node, new Point2D(x, y + 1));
        TryAddRoad(node, new Point2D(x - 1, y));
        TryAddRoad(node, new Point2D(x + 1, y));
    }
        
    private void TryAddRoad(Point2D from, Point2D to)
    {
        if (_graph.ContainsNode(from) && _graph.ContainsNode(to))
        {
            _graph.AddEdge(from, to);
        }
    }
    
    private Vector3 ToVector3(Point2D point)
    {
        return new Vector3(point.x, 0f, point.y) * _gridSize;
    }
    
    public void Remove(Func<Vector3, bool> check)
    {
        var nodesCopy = new List<Point2D>(_graph.nodes);
        foreach (var node in nodesCopy)
        {
            if (check(ToVector3(node)))
            {
                _graph.RemoveNode(node);
            }
        }
    }
}