using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Graph<TNodeData, TEdgeData>
{
    private IDictionary<Vector3, Node<TNodeData>> _nodeMap { get; set; }
    private IDictionary<Vector3, IDictionary<Vector3, Edge<TNodeData, TEdgeData>>> _edgeMap { get; set; }

    private ICollection<Node<TNodeData>> _nodes { get; set; }    
    private IDictionary<Node<TNodeData>, IList<Edge<TNodeData, TEdgeData>>> _adjacentEdges { get; set; }
        
    public Graph()
    {
        _nodeMap = new Dictionary<Vector3, Node<TNodeData>>();
        _edgeMap = new Dictionary<Vector3, IDictionary<Vector3, Edge<TNodeData, TEdgeData>>>();

        _nodes = new List<Node<TNodeData>>();        
        _adjacentEdges = new Dictionary<Node<TNodeData>, IList<Edge<TNodeData, TEdgeData>>>();
    }
    
    public Graph(Graph<TNodeData, TEdgeData> other)
        : this()
    {
        foreach (var node in other.GetNodes())
        {
            AddNode(node);
        }
        foreach (var edge in other.GetEdges())
        {
            AddDirectedEdge(edge);
        }
    }
    
    public Node<TNodeData>[] GetNodes()
    {
        return _nodes.ToArray();
    }
    
    public bool ContainsNode(Vector3 pos)
    {
        return _nodeMap.ContainsKey(pos);
    }
    
    public bool ContainsNode(Node<TNodeData> node)
    {
        return _nodes.Contains(node);
    }
    
    public void AddNode(Vector3 pos, TNodeData data)
    {
        var node = new Node<TNodeData>(pos, data);
        AddNode(node);
    }
    
    public void AddNode(Node<TNodeData> node)
    {
        if (!ContainsNode(node))
        {
            _nodeMap.Add(node.pos, node);
            _edgeMap.Add(node.pos, new Dictionary<Vector3, Edge<TNodeData, TEdgeData>>());
            
            _nodes.Add(node);
            _adjacentEdges.Add(node, new List<Edge<TNodeData, TEdgeData>>());
        }
    }
    
    public void RemoveNode(Vector3 pos)
    {
        if (_nodeMap.ContainsKey(pos))
        {
            RemoveNode(_nodeMap[pos]);
        }
    }
    
    public void RemoveNode(Node<TNodeData> node)
    {
        if (ContainsNode(node))
        {
            _nodeMap.Remove(node.pos);
            _edgeMap.Remove(node.pos);
            
            _nodes.Remove(node);
            _adjacentEdges.Remove(node);
            
            foreach (var edge in GetEdges().Where(e => e.to == node))
            {
                RemoveDirectedEdge(edge);
            }
            foreach (var other in GetNodes())
            {
                _edgeMap[other.pos].Remove(node.pos);
            }
        }
    }
    
    public Edge<TNodeData, TEdgeData>[] GetEdges()
    {
        return _adjacentEdges.Values.SelectMany(e => e).ToArray();
    }
    
    public Edge<TNodeData, TEdgeData> GetEdge(Node<TNodeData> from, Node<TNodeData> to)
    {        
        return GetEdge(from.pos, to.pos);
    }
    
    public Edge<TNodeData, TEdgeData> GetEdge(Vector3 from, Vector3 to)
    {
        if (!_edgeMap.ContainsKey(from) || !_edgeMap[from].ContainsKey(to))
        {
            throw new ArgumentException("No such edge exists.");
        }
        
        return _edgeMap[from][to];
    }
        
    public bool ContainsEdge(Edge<TNodeData, TEdgeData> edge)
    {
        return ContainsEdge(edge.from, edge.to);
    }
    
    public bool ContainsEdge(Node<TNodeData> from, Node<TNodeData> to)
    {
        return ContainsEdge(from.pos, to.pos);
    }
    
    public bool ContainsEdge(Vector3 from, Vector3 to)
    {
        return _edgeMap.ContainsKey(from) && _edgeMap[from].ContainsKey(to);
    }
    
    public void AddUndirectedEdge(Edge<TNodeData, TEdgeData> edge)
    {
        AddUndirectedEdge(edge.from, edge.to, edge.data);
    }
    
    public void AddUndirectedEdge(Node<TNodeData> first, Node<TNodeData> second, TEdgeData data)
    {
        AddUndirectedEdge(first.pos, second.pos, data);
    }
    
    public void AddUndirectedEdge(Vector3 first, Vector3 second, TEdgeData data)
    {
        AddDirectedEdge(first, second, data);  
        AddDirectedEdge(second, first, data);      
    }
    
    public void RemoveUndirectedEdge(Edge<TNodeData, TEdgeData> edge)
    {
        RemoveUndirectedEdge(edge.from, edge.to);
    }
        
    public void RemoveUndirectedEdge(Node<TNodeData> first, Node<TNodeData> second)
    {
        RemoveUndirectedEdge(first.pos, second.pos);
    }
    
    public void RemoveUndirectedEdge(Vector3 first, Vector3 second)
    {
        RemoveDirectedEdge(first, second);
        RemoveDirectedEdge(second, first);
    }
        
    public void AddDirectedEdge(Vector3 from, Vector3 to, TEdgeData data)
    {
        if (ContainsNode(from) && ContainsNode(to))
        {
            AddDirectedEdge(_nodeMap[from], _nodeMap[to], data);
        }
    }
        
    public void AddDirectedEdge(Node<TNodeData> from, Node<TNodeData> to, TEdgeData data)
    {
        var edge = new Edge<TNodeData, TEdgeData>(from, to, data);
        AddDirectedEdge(edge);
    }
    
    public void AddDirectedEdge(Edge<TNodeData, TEdgeData> edge)
    {
        if (ContainsNode(edge.from) && ContainsNode(edge.to) && !ContainsEdge(edge))
        {
            _edgeMap[edge.from.pos].Add(edge.to.pos, edge);
            _adjacentEdges[edge.from].Add(edge);
        }
    }
    
    public void RemoveDirectedEdge(Node<TNodeData> from, Node<TNodeData> to)
    {
        if (ContainsEdge(from, to))
        {
            RemoveDirectedEdge(from.pos, to.pos);
        }
    }
    
    public void RemoveDirectedEdge(Vector3 from, Vector3 to)
    {
        if (ContainsEdge(from, to))
        {
            RemoveDirectedEdge(_edgeMap[from][to]);
        }
    }
    
    public void RemoveDirectedEdge(Edge<TNodeData, TEdgeData> edge)
    {
        if (ContainsEdge(edge))
        {
            _edgeMap[edge.from.pos].Remove(edge.to.pos);
            _adjacentEdges[edge.from].Remove(edge);
        }
    }
    
    public Node<TNodeData>[] GetAdjacentNodes(Node<TNodeData> node)
    {
        if (!ContainsNode(node))
        {
            throw new ArgumentException("Cannot get adjacent nodes for unknown node.");
        }
        
        var adjacentNodes = _adjacentEdges[node].Select(e => e.to).ToArray();
        
        return adjacentNodes;
    }
    
    public Edge<TNodeData, TEdgeData>[] GetOutEdges(Node<TNodeData> node)
    {
        if (!ContainsNode(node))
        {
            throw new ArgumentException("Cannot get out-edges for unknown node.");
        }
        
        var outEdges = _adjacentEdges[node].ToArray();
        
        return outEdges;
    }
}
