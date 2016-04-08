using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class AddRiverStep : GenerationStepBase
{       
    // Bezier control points
    private Vector3 _startPoint;
    private Vector3 _controlPoint1;
    private Vector3 _controlPoint2;
    private Vector3 _endPoint;
    
    public override void Run()
    {        
        PickBezierPoints();
        CalculateSegmentPoints();
    }
    
    private void PickBezierPoints()
    {
        do
        {   
            _startPoint = GetPointInUnitCircle();
            _endPoint = GetPointInUnitCircle();
        }
        while (Vector3.Angle(_startPoint, _endPoint) < 90);
        
        var offset = new Vector3(options.cityWidth / 2f, 0f, options.cityHeight / 2f);
        _startPoint = _startPoint * offset.magnitude + offset;
        _endPoint = _endPoint * offset.magnitude + offset;

        _controlPoint1 = GetRandomPoint();
        _controlPoint2 = GetRandomPoint();
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
    
    private Vector3 GetPointInUnitCircle()
    {
        var point = Random.insideUnitCircle.normalized;
        return new Vector3(point.x, 0f, point.y);
    }
    
    private Vector3 GetRandomPoint()
    {
        var x = Random.Range(0.2f, 0.8f) * options.cityWidth;
        var y = Random.Range(0.2f, 0.8f) * options.cityHeight;
        return new Vector3(x, 0f, y);
    }
}
