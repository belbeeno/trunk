using UnityEngine;

public class Edge<TNodeData, TEdgeData>
{
    public Edge(Node<TNodeData> from, Node<TNodeData> to, TEdgeData data)
    {
        this.from = from;
        this.to = to;
        this.direction = (to.pos - from.pos).normalized;
        this.center = (from.pos + to.pos) / 2f;
        this.length = Vector3.Distance(from.pos, to.pos);
        this.data = data;
    }
    
    public Edge<TNodeData, TEdgeData> Reversed()
    {
        return new Edge<TNodeData, TEdgeData>(to, from, data);
    }
    
    public readonly Node<TNodeData> from;
    public readonly Node<TNodeData> to;
    public readonly TEdgeData data;
    
    public readonly Vector3 direction;
    public readonly Vector3 center;
    public readonly float length;
    
    public static bool operator ==(Edge<TNodeData, TEdgeData> e1, Edge<TNodeData, TEdgeData> e2)
    {
        if (object.ReferenceEquals(e1, e2)) { return true; }
        if (object.ReferenceEquals(e1, null)) { return false; }
        if (object.ReferenceEquals(null, e2)) { return false; }
        return (e1.from == e2.from && e1.to == e2.to);
    }
    
    public static bool operator !=(Edge<TNodeData, TEdgeData> e1, Edge<TNodeData, TEdgeData> e2)
    {
        return !(e1 == e2);
    }
    
    public bool Equals(Edge<TNodeData, TEdgeData> other)
    {            
        return (this == other);
    }

    public override bool Equals(object obj)
    {
        return (obj is Edge<TNodeData, TEdgeData>) && (this == (Edge<TNodeData, TEdgeData>)obj);
    }
    
    public override int GetHashCode()
    {
        return from.GetHashCode() ^ (17 * to.GetHashCode());
    }
}
