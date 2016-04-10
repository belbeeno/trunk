using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class River : MonoBehaviour
{
    public IList<Vector3> data;
    
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
            var currentPoint = data[0];
            foreach (var point in data.Skip(1))
            {
                Gizmos.DrawLine(currentPoint, point);
                currentPoint = point;
            }
        }
    }
}
