using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine;

public class AddRiverStep : GenerationStepBase
{       
    // Bezier control points
    private Vector3 _startPoint;
    private Vector3 _controlPoint1;
    private Vector3 _controlPoint2;
    private Vector3 _endPoint;
    
    public override GenerationData Run()
    {        
        PickBezierPoints();
        CalculateSegmentPoints();
        
        return data;
    }
    
    private void PickBezierPoints()
    {
        do
        {
            _startPoint = GetRandomPointOnEdge();
            _endPoint = GetRandomPointOnEdge();
            _controlPoint1 = GetRandomPoint();
            _controlPoint2 = GetRandomPoint();
        } 
        while (PointsAreUnsatisfactory());
    }
    
    private bool PointsAreUnsatisfactory()
    {
        var sameEdge = _startPoint.x == _endPoint.x || _startPoint.z == _endPoint.z;
        var tooClose = Vector3.Distance(_startPoint, _endPoint) < (0.4 * Mathf.Max(options.cityHeight, options.cityWidth));
        
        return sameEdge || tooClose;
    }
    
    private void CalculateSegmentPoints()
    {
        var points = new List<Vector3>();
        var step = 1f / (options.numRiverSegments);
        while (points.Count <= options.numRiverSegments)
        {
            var t = step * points.Count;
            points.Add(Bezier(t));
        }
        
        data.riverPath = points;
    }
    
    private Vector3 Bezier(float t)
    {
        var mT = 1 - t;
        var result 
            =     mT * mT * mT * _startPoint
            + 3 * mT * mT *  t * _controlPoint1
            + 3 * mT *  t *  t * _controlPoint2 
            +      t *  t *  t * _endPoint;
        
        return result;
    }
    
    private Vector3 GetRandomPointOnEdge()
    {
        Func<bool> tossCoin = () => (Random.Range(0f, 1f) > 0.5f);
        
        if (tossCoin())
        {
            var x = tossCoin() ? 0f : options.cityWidth;
            var y = Random.Range(0.1f, 0.9f) * options.cityHeight;
            return new Vector3(x, 0f, y);
        }
        else
        {
            var x = Random.Range(0.1f, 0.9f) * options.cityWidth;
            var y = tossCoin() ? 0f : options.cityHeight;
            return new Vector3(x, 0f, y);
        }
    }
    
    private Vector3 GetRandomPoint()
    {
        var x = Random.Range(0.2f, 0.8f) * options.cityWidth;
        var y = Random.Range(0.2f, 0.8f) * options.cityHeight;
        return new Vector3(x, 0f, y);
    }
}
