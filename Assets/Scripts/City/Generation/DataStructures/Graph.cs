using System;
using System.Collections.Generic;
using UnityEngine;

public class Edge<TNode>
{
    public Edge(TNode from, TNode to)
    {
        this.from = from;
        this.to = to;    
    }
    
    public TNode from;
    public TNode to;
}

public class Graph<TNode>
{
    public IList<TNode> nodes { get; private set; }    
    private IDictionary<TNode, IList<TNode>> _neighbours;
    
    public Graph()
    {
        nodes = new List<TNode>();
        _neighbours = new Dictionary<TNode, IList<TNode>>();
    }
    
    public void AddNode(TNode node)
    {        
        if (!ContainsNode(node))
        {
            nodes.Add(node);
            _neighbours.Add(node, new List<TNode>());
        }
    }
    
    public void RemoveNode(TNode node)
    {
        if (ContainsNode(node))
        {
            nodes.Remove(node);
            _neighbours.Remove(node);
            foreach (var otherNode in nodes)
            {
                _neighbours[otherNode].Remove(node);
            }
        }
    }
    
    public bool ContainsNode(TNode node)
    {
        return nodes.Contains(node);
    }
    
    public IList<Edge<TNode>> edges
    {
        get
        {
            var edges = new List<Edge<TNode>>();
            foreach (var fromNode in nodes)
            foreach (var toNode in _neighbours[fromNode])
            {
                edges.Add(new Edge<TNode>(fromNode, toNode));
            }
            return edges;
        }
    }
        
    public void AddEdge(TNode first, TNode second)
    {
        AddDirectedEdge(first, second);
        AddDirectedEdge(second, first);
    }
    
    public void AddDirectedEdge(TNode first, TNode second)
    {
        if (!nodes.Contains(first) || !nodes.Contains(second))
        {
            throw new ArgumentException("Cannot add edge between unknown nodes");
        }
                
        if (!_neighbours[first].Contains(second))
        {
            _neighbours[first].Add(second);
        }
    }
    
    public void RemoveEdge(TNode first, TNode second)
    {
        RemoveDirectedEdge(first, second);
        RemoveDirectedEdge(second, first);
    }
    
    public void RemoveDirectedEdge(TNode first, TNode second)
    {
        if (nodes.Contains(first) && nodes.Contains(second))
        {
            _neighbours[first].Remove(second);
        }
    }
    
    public bool ContainsEdge(TNode first, TNode second)
    {
        if (!nodes.Contains(first) || !nodes.Contains(second))
        {
            throw new ArgumentException("Cannot check for edge between unknown nodes");
        }
        
        return _neighbours[first].Contains(second);
    }
}
