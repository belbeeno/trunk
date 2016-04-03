using UnityEngine;

public class Road : MonoBehaviour
{
    public RoadData data;
    
    public void Start()
    {
    }
    
    public void Update()
    {
    }

    public void OnDrawGizmos()
    {
        if (data != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(data.from, data.to);
        }
    }
}
