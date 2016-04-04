using System.Collections.Generic;
using UnityEngine;

public class RiverData
{
    public RiverData(IList<Vector3> points)
    {
        this.points = points;
    }
    
    public IList<Vector3> points;
}