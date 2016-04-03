using System.Linq;
using UnityEngine;

public class River : MonoBehaviour
{
    public RiverData data;
    
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
            Gizmos.color = Color.blue;
            var currentPoint = data.points[0];
            foreach (var point in data.points.Skip(1))
            {
                Gizmos.DrawLine(currentPoint, point);
                currentPoint = point;
            }
        }
    }
}
