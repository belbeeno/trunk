public class RiverEdgeData
{
}

public class RiverEdge : Edge<RiverNode, RiverNodeData, RiverEdgeData>
{
    public RiverEdge(RiverNode from, RiverNode to, RiverEdgeData data)
        : base(from, to, data)
    {
    }
}
