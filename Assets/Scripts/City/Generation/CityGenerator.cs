using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CityGenerationOptions
{
    [Range(0, 32)] public int width;
    [Range(0, 32)] public int height;
    
    [Range(0f, 8f)] public float floorHeight;
    [Range(4f, 8f)] public float blockSize;
    [Range(1f, 4f)] public float roadWidth;
    
    [Range(0f, 8f)] public float riverWidth;
    [Range(0, 32)] public int riverControlPoints;
    [Range(0f, 16f)] public float riverMaxDeviation;   
}

public class CityGenerationResult
{
    public IRoadGraph roadGraph { get; set; }
    public ICityPlan cityPlan { get; set; }
    public IRiverLayout riverPath { get; set; }
}

public class CityGenerator
{
    private CityGenerationOptions _options;
    
    private GridRoadGraph _roadGraph;
    private GridCityPlan _cityPlan;
    private BezierRiver _riverPath;
    
    public CityGenerationResult Generate(CityGenerationOptions options)
    {
        _options = options;
        
        GenerateCity();
        GenerateRiver();
        
        var results = new CityGenerationResult
            {
                roadGraph = _roadGraph,
                cityPlan = _cityPlan,
                riverPath = _riverPath
            };
            
        return results;
    }
    
    private void GenerateCity()
    {
        _roadGraph = new GridRoadGraph(_options.blockSize);
        _cityPlan = new GridCityPlan(_options.blockSize, _options.roadWidth, _options.floorHeight);
        
        for (var x = 0; x <= _options.width; x++)
        for (var y = 0; y <= _options.height; y++)
        {
            if (x > 0 && y > 0)
            {
                _cityPlan.AddCityBlock(x - 1, y - 1);
            }
            _roadGraph.AddIntersection(x, y);
        }
    }
    
    private void GenerateRiver()
    {
        var cityWidth = _options.blockSize * _options.width;
        var cityHeight = _options.blockSize * _options.height;
        _riverPath = new BezierRiver(cityWidth, cityHeight, _options.riverWidth, _options.riverControlPoints);
        _riverPath.Generate();
        
        _roadGraph.Remove(_riverPath.IsInRiver);
        _cityPlan.Remove(_riverPath.IsInRiver);
    }
}