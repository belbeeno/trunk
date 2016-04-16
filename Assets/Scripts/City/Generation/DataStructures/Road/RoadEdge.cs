public class RoadEdgeData
{
    public bool isBridge;
    
    public RoadEdgeData(bool isBridge)
    {
        this.isBridge = isBridge;
    }
}

public class RoadEdge : Edge<RoadNode, RoadNodeData, RoadEdgeData>
{
    public RoadEdge(RoadNode from, RoadNode to, RoadEdgeData data)
        : base(from, to, data)
    {
    }
}
