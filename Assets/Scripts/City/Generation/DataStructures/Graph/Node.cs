using UnityEngine;

public abstract class Node<TData>
{
    public readonly Vector3 pos;
    public readonly TData data;
    
    public Node(Vector3 pos, TData data)
    {
        this.pos = pos;
        this.data = data;
    }
    
    public static bool operator ==(Node<TData> n1, Node<TData> n2)
    {    
        if (object.ReferenceEquals(n1, n2)) { return true; }
        if (object.ReferenceEquals(n1, null)) { return false; }
        if (object.ReferenceEquals(null, n2)) { return false; }
        return n1.pos == n2.pos;
    }
    
    public static bool operator !=(Node<TData> n1, Node<TData> n2)
    {
        return !(n1 == n2);
    }
    
    public bool Equals(Node<TData> other)
    {            
        return (this == other);
    }

    public override bool Equals(object obj)
    {
        return (obj is Node<TData>) && (this == (Node<TData>)obj);
    }
    
    public override int GetHashCode()
    {
        return pos.GetHashCode();
    }
}
