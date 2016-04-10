using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VectorExtensions
{
    
    public static Vector3 Sum(this IEnumerable<Vector3> source)
    {
        var result = source.Aggregate(Vector3.zero, (a, v) => a + v);
        
        return result;
    }
    public static Vector3 Average(this IEnumerable<Vector3> source)
    {
        var result = source.Sum() / source.Count();
        
        return result;
    }
}
