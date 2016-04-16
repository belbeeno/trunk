using UnityEngine;

public class RiverNodeData
{
}

public class RiverNode : Node<RiverNodeData>
{
    public RiverNode(Vector3 pos, RiverNodeData data)
        : base(pos, data)
    {
    }
}
