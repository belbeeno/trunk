using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine;

public class CityPlan
{
    private float _blockSize;
    private float _roadWidth;
    private float _floorHeight;
    
    private IList<CityBlockData> _cityBlocks;
    
    public CityPlan(float blockSize, float roadWidth, float floorHeight)
    {
        _blockSize = blockSize;
        _roadWidth = roadWidth;
        _floorHeight = floorHeight;
        
        _cityBlocks = new List<CityBlockData>();
    }
        
    public void AddCityBlock(int x, int y)
    {
        var corners = GetCityBlockCorners(x, y);
        var numFloors = Random.Range(1, 6);
        var cityBlock = new CityBlockData(corners, numFloors, _floorHeight);
        _cityBlocks.Add(cityBlock);
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
        var cityBlocksCopy = new List<CityBlockData>(_cityBlocks);
        foreach (var cityBlock in cityBlocksCopy)
        foreach (var corner in cityBlock.corners)
        {
            if (check(corner))
            {
                _cityBlocks.Remove(cityBlock);
                break;
            }
        }
    }
    
    public CityBlockData[] GetCityBlocks()
    {
        return _cityBlocks.ToArray();
    }
}