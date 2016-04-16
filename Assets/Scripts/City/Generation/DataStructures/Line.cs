using System;
using UnityEngine;

public class Line
{
    private Vector3 pos;
    private Vector3 dir;
    
    private Line(Vector3 pos, Vector3 dir) 
    {
        this.pos = pos;
        this.dir = dir;    
    }
    
    public static Line Create(Vector3 pos, Vector3 dir)
    {
        return new Line(pos, dir.normalized);
    }
    
    public static Line CreateThroughPoints(Vector3 first, Vector3 second)
    {
        return new Line(first, (second - first).normalized);
    }
    
    public static Vector3 Intersection(Line first, Line second)
    { 
		var a = Vector3.Dot(first.dir, first.dir);
		var b = Vector3.Dot(first.dir, second.dir);
		var e = Vector3.Dot(second.dir, second.dir);
 		var d = a*e - b*b;
 
		if (d == 0f) 
        {
            throw new ArgumentException("Lines are parallel!");
        }
 
		var r = first.pos - second.pos;
		var c = Vector3.Dot(first.dir, r);
		var f = Vector3.Dot(second.dir, r);
 
		var s = (b*f - c*e) / d;
		var t = (a*f - c*b) / d;
 
		var closestPointLine1 = first.pos + s * first.dir;
		var closestPointLine2 = second.pos + t * second.dir;
        var avg = (closestPointLine1 + closestPointLine2) / 2f;
 
		return avg;
	}
    
    public Vector3 Perpendicular()
    {
        return new Vector3(-dir.z, dir.y, dir.x).normalized;
    }
    
    public Line Offset(Vector3 offset)
    {
        return Line.Create(pos + offset, dir);
    }
}
