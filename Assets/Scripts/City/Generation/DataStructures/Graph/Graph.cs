using System;
using System.Collections.Generic;
using System.Linq;

public sealed class Graph<TNodeData, TEdgeData>
{
    private ICollection<Node<TNodeData>> _nodes { get; set; }
    private IDictionary<Node<TNodeData>, IList<Edge<TNodeData, TEdgeData>>> _adjacentEdges { get; set;}
        
    public Graph()
    {
        _nodes = new List<Node<TNodeData>>();
        _adjacentEdges = new Dictionary<Node<TNodeData>, IList<Edge<TNodeData, TEdgeData>>>();
    }
    
    public Node<TNodeData>[] GetNodes()
    {
        return _nodes.ToArray();
    }
    
    public bool ContainsNode(Node<TNodeData> node)
    {
        return _nodes.Contains(node);
    }
    
    public void AddNode(Node<TNodeData> node)
    {        
        if (!ContainsNode(node))
        {
            _nodes.Add(node);
            _adjacentEdges.Add(node, new List<Edge<TNodeData, TEdgeData>>());
        }
    }
    
    public void RemoveNode(Node<TNodeData> node)
    {
        if (ContainsNode(node))
        {
            _nodes.Remove(node);
            _adjacentEdges.Remove(node);
            
            foreach (var edge in GetEdges().Where(e => e.to == node))
            {
                RemoveDirectedEdge(edge);
            }
        }
    }
    
    public Edge<TNodeData, TEdgeData>[] GetEdges()
    {
        return _adjacentEdges.Values.SelectMany(e => e).ToArray();
    }
        
    public bool ContainsEdge(Edge<TNodeData, TEdgeData> edge)
    {
        return ContainsNode(edge.from) && _adjacentEdges[edge.from].Contains(edge);
    }
        
    public void AddUndirectedEdge(Edge<TNodeData, TEdgeData> edge)
    {
        AddDirectedEdge(edge);
        AddDirectedEdge(edge.Reversed());
    }
    
    public void RemoveUndirectedEdge(Edge<TNodeData, TEdgeData> edge)
    {
        RemoveDirectedEdge(edge);
        RemoveDirectedEdge(edge.Reversed());
    }
        
    public void AddDirectedEdge(Edge<TNodeData, TEdgeData> edge)
    {
        if (ContainsNode(edge.from) && ContainsNode(edge.to) && !ContainsEdge(edge))
        {
            _adjacentEdges[edge.from].Add(edge);
        }
    }
    
    public void RemoveDirectedEdge(Edge<TNodeData, TEdgeData> edge)
    {
        if (ContainsEdge(edge))
        {
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
