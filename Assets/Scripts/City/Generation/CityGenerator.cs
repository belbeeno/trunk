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
}

public class CityGenerationResult
{
    public IRoadGraph roadGraph { get; set; }
    public ICityPlan cityPlan { get; set; }
}

public class CityGenerator
{
    private CityGenerationOptions _options;
    
    private GridRoadGraph _roadGraph;
    private GridCityPlan _cityPlan;
    
    public CityGenerationResult Generate(CityGenerationOptions options)
    {
        _options = options;
        
        GenerateCity();
        
        var results = new CityGenerationResult
            {
                roadGraph = _roadGraph,
                cityPlan = _cityPlan
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
}