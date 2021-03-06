using UnityEngine;

public class RoadGraph : Graph<RoadNode, RoadNodeData, RoadEdge, RoadEdgeData>
{
    public RoadGraph()
        : base((p,d) => new RoadNode(p,d), (f,t,d) => new RoadEdge(f,t,d))
    {
    }
    
    public RoadGraph(RoadGraph other)
        : base(other)
    {
    }
    
    public void AddNode(Vector3 pos)
    {
        AddNode(pos, new RoadNodeData());
    }
    
    public void AddUndirectedEdge(Vector3 from, Vector3 to, bool isBridge)
    {
        var fromNode = new RoadNode(from, new RoadNodeData());
        var toNode = new RoadNode(to, new RoadNodeData());
        AddUndirectedEdge(fromNode, toNode, new RoadEdgeData(isBridge));
    }
}
