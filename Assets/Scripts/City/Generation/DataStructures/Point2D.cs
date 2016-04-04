using System;

public struct Point2D : IEquatable<Point2D>
{
    public int x;
    public int y;
    
    public Point2D(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public override string ToString()
    {
        return string.Format("({0},{1})", x, y);
    }
    
    public static bool operator ==(Point2D p1, Point2D p2)
    {    
        return (p1.x == p2.x) && (p1.y == p2.y);
    }
    
    public static bool operator !=(Point2D p1, Point2D p2)
    {
        return !(p1 == p2);
    }
    
    public bool Equals(Point2D other)
    {            
        return (this == other);
    }

    public override bool Equals(object obj)
    {
        return obj is Point2D && (this == (Point2D)obj);
    }
    
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 237 + x.GetHashCode();
            hash = hash * 23942 + y.GetHashCode();
            return hash;
        }
    }
}