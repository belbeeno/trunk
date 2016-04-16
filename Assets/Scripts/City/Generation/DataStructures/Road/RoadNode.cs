using UnityEngine;

public class RoadNodeData
{
}

public class RoadNode : Node<RoadNodeData>
{
    public RoadNode(Vector3 pos, RoadNodeData data)
        : base(pos, data)
    {
    }
}
