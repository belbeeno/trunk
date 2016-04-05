using UnityEngine;

public class GenerateGridStep : GenerationStepBase
{    
    public override GenerationData Run()
    {                
        for (var x = 0; x <= options.blocksWidth; x++)
        for (var y = 0; y <= options.blocksHeight; y++)
        {
            AddIntersection(x, y);
            
            if (x > 0 && y > 0)
            {
                AddCityBlock(x, y);
            }
        }
        
        return data;
    }
    
    private void AddIntersection(int x, int y)
    {
        var pos = ToVector3(x, y);
        data.roadGraph.AddIntersection(pos);
        data.roadGraph.AddRoad(pos, ToVector3(x, y - 1));
        data.roadGraph.AddRoad(pos, ToVector3(x - 1, y));
    }
    
    private void AddCityBlock(int x, int y)
    {
        var offset = new Vector3(-0.5f, 0f, -0.5f) * options.blockSize;
        var pos = ToVector3(x, y) + offset;
        data.cityPlan.AddCityBlock(pos);
    }
    
    private Vector3 ToVector3(int x, int y)
    {
        return new Vector3(x, 0f, y) * options.blockSize;
    }
}