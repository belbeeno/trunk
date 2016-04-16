using UnityEngine;

public class RiverGraph : Graph<RiverNode, RiverNodeData, RiverEdge, RiverEdgeData>
{
    public RiverGraph()
        : base((p,d) => new RiverNode(p,d), (f,t,d) => new RiverEdge(f,t,d))
    {
    }
    
    public RiverGraph(RiverGraph other)
        : base(other)
    {
    }
    
    public void AddNode(Vector3 pos)
    {
        AddNode(pos, new RiverNodeData());
    }
    
    public void AddDirectedEdge(Vector3 from, Vector3 to)
    {
        var fromNode = new RiverNode(from, new RiverNodeData());
        var toNode = new RiverNode(to, new RiverNodeData());
        AddDirectedEdge(fromNode, toNode, new RiverEdgeData());
    }
}
