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
                var from = new Vector3(edge.from.x, 0f, edge.from.y) * _gridSize;
                var to = new Vector3(edge.to.x, 0f, edge.to.y) * _gridSize;
                roads.Add(new RoadData(from, to));
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
}