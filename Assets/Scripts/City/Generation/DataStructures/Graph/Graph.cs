using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Graph<TNode, TNodeData, TEdge, TEdgeData>
    where TNode : Node<TNodeData>
    where TEdge : Edge<TNode, TNodeData, TEdgeData>
{
    private Func<Vector3, TNodeData, TNode> _nodeFactory;
    private Func<TNode, TNode, TEdgeData, TEdge> _edgeFactory;
    
    private IDictionary<Vector3, TNode> _nodeMap { get; set; }
    private IDictionary<Vector3, IDictionary<Vector3, TEdge>> _edgeMap { get; set; }

    private ICollection<TNode> _nodes { get; set; }    
    private IDictionary<TNode, IList<TEdge>> _adjacentEdges { get; set; }
        
    public Graph(Func<Vector3, TNodeData, TNode> nodeFactory, Func<TNode, TNode, TEdgeData, TEdge> edgeFactory)
    {
        _nodeFactory = nodeFactory;
        _edgeFactory = edgeFactory;
        
        _nodeMap = new Dictionary<Vector3, TNode>();
        _edgeMap = new Dictionary<Vector3, IDictionary<Vector3, TEdge>>();

        _nodes = new List<TNode>();        
        _adjacentEdges = new Dictionary<TNode, IList<TEdge>>();
    }
    
    public Graph(Graph<TNode, TNodeData, TEdge, TEdgeData> other)
        : this(other._nodeFactory, other._edgeFactory)
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
    
    public TNode[] GetNodes()
    {
        return _nodes.ToArray();
    }
    
    public bool ContainsNode(Vector3 pos)
    {
        return _nodeMap.ContainsKey(pos);
    }
    
    public bool ContainsNode(TNode node)
    {
        return _nodes.Contains(node);
    }
    
    public void AddNode(Vector3 pos, TNodeData data)
    {
        var node = _nodeFactory(pos, data);
        AddNode(node);
    }
    
    public void AddNode(TNode node)
    {
        if (!ContainsNode(node))
        {
            _nodeMap.Add(node.pos, node);
            _edgeMap.Add(node.pos, new Dictionary<Vector3, TEdge>());
            
            _nodes.Add(node);
            _adjacentEdges.Add(node, new List<TEdge>());
        }
    }
    
    public void RemoveNode(Vector3 pos)
    {
        if (_nodeMap.ContainsKey(pos))
        {
            RemoveNode(_nodeMap[pos]);
        }
    }
    
    public void RemoveNode(TNode node)
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
    
    public TEdge[] GetEdges()
    {
        return _adjacentEdges.Values.SelectMany(e => e).ToArray();
    }
    
    public TEdge GetEdge(Node<TNodeData> from, Node<TNodeData> to)
    {        
        return GetEdge(from.pos, to.pos);
    }
    
    public TEdge GetEdge(Vector3 from, Vector3 to)
    {
        if (!_edgeMap.ContainsKey(from) || !_edgeMap[from].ContainsKey(to))
        {
            throw new ArgumentException("No such edge exists.");
        }
        
        return _edgeMap[from][to];
    }
        
    public bool ContainsEdge(TEdge edge)
    {
        return ContainsEdge(edge.from, edge.to);
    }
    
    public bool ContainsEdge(TNode from, TNode to)
    {
        return ContainsEdge(from.pos, to.pos);
    }
    
    public bool ContainsEdge(Vector3 from, Vector3 to)
    {
        return _edgeMap.ContainsKey(from) && _edgeMap[from].ContainsKey(to);
    }
    
    public void AddUndirectedEdge(TEdge edge)
    {
        AddUndirectedEdge(edge.from, edge.to, edge.data);
    }
    
    public void AddUndirectedEdge(TNode first, TNode second, TEdgeData data)
    {
        AddUndirectedEdge(first.pos, second.pos, data);
    }
    
    public void AddUndirectedEdge(Vector3 first, Vector3 second, TEdgeData data)
    {
        AddDirectedEdge(first, second, data);  
        AddDirectedEdge(second, first, data);      
    }
    
    public void RemoveUndirectedEdge(TEdge edge)
    {
        RemoveUndirectedEdge(edge.from, edge.to);
    }
        
    public void RemoveUndirectedEdge(TNode first, TNode second)
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
        
    public void AddDirectedEdge(TNode from, TNode to, TEdgeData data)
    {
        var edge = _edgeFactory(from, to, data);
        AddDirectedEdge(edge);
    }
    
    public void AddDirectedEdge(TEdge edge)
    {
        if (ContainsNode(edge.from) && ContainsNode(edge.to) && !ContainsEdge(edge))
        {
            _edgeMap[edge.from.pos].Add(edge.to.pos, edge);
            _adjacentEdges[edge.from].Add(edge);
        }
    }
    
    public void RemoveDirectedEdge(TNode from, TNode to)
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
    
    public void RemoveDirectedEdge(TEdge edge)
    {
        if (ContainsEdge(edge))
        {
            _edgeMap[edge.from.pos].Remove(edge.to.pos);
            _adjacentEdges[edge.from].Remove(edge);
        }
    }
    
    public TNode[] GetAdjacentNodes(TNode node)
    {
        if (!ContainsNode(node))
        {
            throw new ArgumentException("Cannot get adjacent nodes for unknown node.");
        }
        
        var adjacentNodes = _adjacentEdges[node].Select(e => e.to).ToArray();
        
        return adjacentNodes;
    }
    
    public TEdge[] GetOutEdges(TNode node)
    {
        if (!ContainsNode(node))
        {
            throw new ArgumentException("Cannot get out-edges for unknown node.");
        }
        
        var outEdges = _adjacentEdges[node].ToArray();
        
        return outEdges;
    }
}
