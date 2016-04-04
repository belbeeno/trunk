public class GenerateGridStep : IGenerationStep
{
    public GenerationData Run(GenerationOptions options, GenerationData data)
    {
        data.roadGraph = new RoadGraph(options.blockSize);
        data.cityPlan = new CityPlan(options.blockSize, options.roadWidth, options.floorHeight);
        
        for (var x = 0; x <= options.width; x++)
        for (var y = 0; y <= options.height; y++)
        {
            if (x > 0 && y > 0)
            {
                data.cityPlan.AddCityBlock(x - 1, y - 1);
            }
            data.roadGraph.AddIntersection(x, y);
        }
        
        return data;
    }
}