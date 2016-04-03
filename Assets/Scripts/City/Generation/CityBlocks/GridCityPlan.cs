using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine;

public class GridCityPlan : ICityPlan
{
    private float _blockSize;
    private float _roadWidth;
    private float _floorHeight;
    private IList<CityBlockData> _plots;
    
    public GridCityPlan(float blockSize, float roadWidth, float floorHeight)
    {
        _blockSize = blockSize;
        _roadWidth = roadWidth;
        _floorHeight = floorHeight;
        
        _plots = new List<CityBlockData>();
    }
    
    public IEnumerable<CityBlockData> plots { get { return _plots.ToArray(); } }
    
    public void AddCityBlock(int x, int y)
    {
        var corners = GetCityBlockCorners(x, y);
        var numFloors = Random.Range(1, 6);
        var plot = new CityBlockData(corners, numFloors, _floorHeight);
        _plots.Add(plot);
    }
        
    private Vector3[] GetCityBlockCorners(int x, int y)
    {
        var centerX = (x + 0.5f) * _blockSize;
        var centerZ = (y + 0.5f) * _blockSize;
        var center = new Vector3(centerX, 0f, centerZ);
         
        var offset = (_blockSize - _roadWidth) / 2;
        var corners = new[] 
            {
                center + new Vector3(offset, 0f, offset),
                center + new Vector3(-offset, 0f, offset),
                center + new Vector3(-offset, 0f, -offset),
                center + new Vector3(offset, 0f, -offset),
            };
         
        return corners;
    }
    
    public void Remove(Func<Vector3, bool> check)
    {
        var plotsCopy = new List<CityBlockData>(_plots);
        foreach (var plot in plotsCopy)
        foreach (var corner in plot.corners)
        {
            if (check(corner))
            {
                _plots.Remove(plot);
                break;
            }
        }
    }
}