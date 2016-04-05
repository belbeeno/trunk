using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine;

public class CityPlan
{
    private GenerationOptions _options;
    
    private IList<CityBlockData> _cityBlocks;
    
    public CityPlan(GenerationOptions options)
    {
        _options = options;
        _cityBlocks = new List<CityBlockData>();
    }
        
    public void AddCityBlock(Vector3 cityBlockPos)
    {
        var corners = GetCityBlockCorners(cityBlockPos);
        var numFloors = Random.Range(1, 6);
        var cityBlock = new CityBlockData(cityBlockPos, corners, numFloors, _options.floorHeight);
        _cityBlocks.Add(cityBlock);
    }
        
    private Vector3[] GetCityBlockCorners(Vector3 cityBlockPos)
    {         
        var offset = ((1f - _options.roadWidth) * _options.blockSize) / 2;
        var corners = new[] 
            {
                cityBlockPos + new Vector3(offset, 0f, offset),
                cityBlockPos + new Vector3(-offset, 0f, offset),
                cityBlockPos + new Vector3(-offset, 0f, -offset),
                cityBlockPos + new Vector3(offset, 0f, -offset),
            };
         
        return corners;
    }
    
    public void RemoveCityBlockWhere(Func<CityBlockData, bool> check)
    {
        var cityBlocksCopy = new List<CityBlockData>(_cityBlocks);
        foreach (var cityBlock in cityBlocksCopy.Where(check))
        {
            _cityBlocks.Remove(cityBlock);
        }
    }
    
    public CityBlockData[] GetCityBlocks()
    {
        return _cityBlocks.ToArray();
    }
}