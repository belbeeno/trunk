using UnityEngine;

public class River : MonoBehaviour
{
    public RiverGraph riverGraph;
    
    public void Start()
    {
    }
    
    public void Update()
    {
    }

    public void OnDrawGizmos()
    {
        if (riverGraph != null)
        {
            Gizmos.color = Color.blue;
            riverGraph.GetEdges()
                .ForEach(e => Gizmos.DrawLine(e.from.pos, e.to.pos));
        }
    }
}
