using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class AddRiverStep : IGenerationStep
{
    // City parameters
    private float _cityWidth;
    private float _cityHeight;
    
    // Bezier control points
    private Vector3 _startPoint;
    private Vector3 _controlPoint1;
    private Vector3 _controlPoint2;
    private Vector3 _endPoint;
    
    // Generation data
    private GenerationOptions _options;
    private GenerationData _data;
    
    public GenerationData Run(GenerationOptions options, GenerationData data)
    {
        _cityWidth = options.blockSize * options.width;
        _cityHeight = options.blockSize * options.height;
        _data = data;
        _options = options;
        
        PickBezierPoints();
        CalculateSegmentPoints();
        RemoveRoadsAndBlocksNearRiver();
        
        return data;
    }
    
    private void PickBezierPoints()
    {
        do
        {
            _startPoint = GetRandomPointOnEdge();
            _endPoint = GetRandomPointOnEdge();
        } 
        while (EndpointsAreUnsatisfactory());
        
        _controlPoint1 = GetRandomPoint();
        _controlPoint2 = GetRandomPoint();
    }
    
    private bool EndpointsAreUnsatisfactory()
    {
        var sameEdge = _startPoint.x == _endPoint.x || _startPoint.z == _endPoint.z;
        var tooClose = Vector3.Distance(_startPoint, _endPoint) < (0.25 * (_cityHeight + _cityWidth));
        return sameEdge || tooClose;
    }
    
    private void CalculateSegmentPoints()
    {
        var points = new List<Vector3>();
        var step = 1f / (_options.numRiverSegments);
        while (points.Count <= _options.numRiverSegments)
        {
            var t = step * points.Count;
            points.Add(Bezier(t));
        }
        
        _data.riverPath = points;
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
    
    private Vector3 GetRandomPoint()
    {
        var x = Random.Range(0.2f, 0.8f) * _cityWidth;
        var y = Random.Range(0.2f, 0.8f) * _cityHeight;
        return new Vector3(x, 0f, y);
    }
    
    private void RemoveRoadsAndBlocksNearRiver()
    {        
        _data.roadGraph.Remove(IsInRiver);
        _data.cityPlan.Remove(IsInRiver);
    }
    
    private bool IsInRiver(Vector3 point)
    {
        foreach (var testPoint in _data.riverPath)
        {
            if (Vector3.Distance(point, testPoint) < (_options.riverWidth / 2))
            {
                return true;
            }
        }
        
        return false;
    }
}
