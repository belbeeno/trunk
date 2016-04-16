using UnityEngine;

public class Road : MonoBehaviour
{
    public RoadEdge road;
    
    public void Start()
    {
    }
    
    public void Update()
    {
    }

    public void OnDrawGizmos()
    {
        if (road != null)
        {
            Gizmos.color = (road.data.isBridge)
                ? Color.green
                : Color.yellow;
            
            Gizmos.DrawLine(road.from.pos, road.to.pos);
        }
    }
}
