using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BezierRiver : IRiverLayout
{    
    private float _cityWidth;
    private float _cityHeight;
    private float _riverWidth;
    private int _numControlPoints;
    
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private Vector3 _riverDir;
    private RiverData _riverData;
    
    public BezierRiver(float cityWidth, float cityHeight, float riverWidth, int numControlPoints)
    {
        _cityWidth = cityWidth;
        _cityHeight = cityHeight;
        _riverWidth = riverWidth;
        _numControlPoints = numControlPoints;
    }
    
    public RiverData river { get { return _riverData; } }
    
    public void Generate()
    {
        do
        {
            _startPoint = GetRandomPointOnEdge();
            _endPoint = GetRandomPointOnEdge();
        } 
        while (PointsAreUnsatisfactory());
        var controlPoint1 = GetRandomPoint();
        var controlPoint2 = GetRandomPoint();
        
        var points = new List<Vector3>();
        var step = 1f / (_numControlPoints - 1);
        while (points.Count < _numControlPoints)
        {
            var t = step * points.Count;
            points.Add(Bezier(t, _startPoint, controlPoint1, controlPoint2, _endPoint));
        }
        
        _riverData = new RiverData(points);
    }
    
    public bool PointsAreUnsatisfactory()
    {
        var sameEdge = _startPoint.x == _endPoint.x || _startPoint.z == _endPoint.z;
        var tooClose = Vector3.Distance(_startPoint, _endPoint) < (0.25 * (_cityHeight + _cityWidth));
        return sameEdge || tooClose;
    }
    
    public Vector3 Bezier(float t, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        var mT = 1 - t;
        var result = mT*mT*mT*p1 + 3*mT*mT*t*p2 + 3*mT*t*t*p3 + t*t*t*p4;
        
        return result;
    }
    
    public Vector3 GetRandomPointOnEdge()
    {
        Func<bool> tossCoin = () => (Random.Range(0f, 1f) > 0.5f);
        
        if (tossCoin())
        {
            var x = tossCoin() ? 0f : _cityWidth;
            var y = Random.Range(0.1f, 0.9f) * _cityHeight;
            return new Vector3(x, 0f, y);
        }
        else
        {
            var x = Random.Range(0.1f, 0.9f) * _cityWidth;
            var y = tossCoin() ? 0f : _cityHeight;
            return new Vector3(x, 0f, y);
        }
    }
    
    public Vector3 GetRandomPoint()
    {
        var x = Random.Range(0.2f, 0.8f) * _cityWidth;
        var y = Random.Range(0.2f, 0.8f) * _cityHeight;
        return new Vector3(x, 0f, y);
    }
    
    public bool IsInRiver(Vector3 point)
    {
        foreach (var testPoint in _riverData.points)
        {
            if (Vector3.Distance(point, testPoint) < (_riverWidth / 2))
            {
                return true;
            }
        }
        
        return false;
    }
}