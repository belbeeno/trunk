using UnityEngine;

public abstract class Edge<TNode, TNodeData, TEdgeData>
    where TNode : Node<TNodeData>
{
    public Edge(TNode from, TNode to, TEdgeData data)
    {
        this.from = from;
        this.to = to;
        this.data = data;
        
        direction = (to.pos - from.pos).normalized;
        center = (from.pos + to.pos) / 2f;
        length = Vector3.Distance(from.pos, to.pos);
    }
        
    public readonly TNode from;
    public readonly TNode to;
    public readonly TEdgeData data;
    
    public readonly Vector3 direction;
    public readonly Vector3 center;
    public readonly float length;
    
    public static bool operator ==(Edge<TNode, TNodeData, TEdgeData> e1, Edge<TNode, TNodeData, TEdgeData> e2)
    {
        if (object.ReferenceEquals(e1, e2)) { return true; }
        if (object.ReferenceEquals(e1, null)) { return false; }
        if (object.ReferenceEquals(null, e2)) { return false; }
        return (e1.from == e2.from && e1.to == e2.to);
    }
    
    public static bool operator !=(Edge<TNode, TNodeData, TEdgeData> e1, Edge<TNode, TNodeData, TEdgeData> e2)
    {
        return !(e1 == e2);
    }
    
    public bool Equals(Edge<TNode, TNodeData, TEdgeData> other)
    {            
        return (this == other);
    }

    public override bool Equals(object obj)
    {
        return (obj is Edge<TNode, TNodeData, TEdgeData>) && (this == (Edge<TNode, TNodeData, TEdgeData>)obj);
    }
    
    public override int GetHashCode()
    {
        return from.GetHashCode() ^ (17 * to.GetHashCode());
    }
}
