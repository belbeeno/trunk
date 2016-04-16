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
    
    public static Vector3[] Inset(this IEnumerable<Vector3> points, float amount)
    {
        // Inset boundaries
        var currentPoint = points.Last();
        var lines = new List<Line>();
        foreach (var point in points)
        {
            lines.Add(Line.CreateThroughPoints(currentPoint, point));
            currentPoint = point;
        }
        var insetLines = lines.Select(l => l.Offset(l.Perpendicular() * amount));
        
        // Find intersections
        var corners = new List<Vector3>();
        var prevLine = insetLines.Last();
        foreach (var line in insetLines)
        {
            corners.Add(Line.Intersection(prevLine, line));
            prevLine = line;    
        }

        return corners.ToArray();
    }
    
    public static Vector3[] Outset(this IEnumerable<Vector3> points, float amount)
    {
        return points.Inset(-amount);
    }
}
