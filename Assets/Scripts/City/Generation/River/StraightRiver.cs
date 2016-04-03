using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StraightRiver : IRiverLayout
{    
    private float _cityWidth;
    private float _cityHeight;
    private float _riverWidth;
    
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private Vector3 _riverDir;
    private RiverData _riverData;
    
    public StraightRiver(float cityWidth, float cityHeight, float riverWidth)
    {
        _cityWidth = cityWidth;
        _cityHeight = cityHeight;
        _riverWidth = riverWidth;
    }
    
    public RiverData river { get { return _riverData; } }
    
    public void Generate()
    {
        do
        {
            _startPoint = GetRandomPointOnEdge();
            _endPoint = GetRandomPointOnEdge();
        } 
        while (_startPoint.x == _endPoint.x || _startPoint.z == _endPoint.z);
              
        _riverDir = (_endPoint - _startPoint);
        _riverData = new RiverData(new[] { _startPoint, _endPoint });
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
    
    public bool IsInRiver(Vector3 point)
    {
         var distanceFromStart = Vector3.Distance(_startPoint, point);
         var angleFromStart = Vector3.Angle(_riverDir, point - _startPoint);
         var distanceToRiver = Mathf.Abs((distanceFromStart * Mathf.Sin(angleFromStart * Mathf.Deg2Rad)));
         var inRiver = distanceToRiver <= (0.5 * _riverWidth);
         
         return inRiver;
    }
}