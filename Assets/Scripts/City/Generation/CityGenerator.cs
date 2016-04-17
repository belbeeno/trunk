public class CityGenerator
{    
    private IGenerationStep[] _steps = new IGenerationStep[] 
        {
            new GenerateGridStep(),
            new AddRiverStep(),
            new ClearAreaNearRiverStep(),
            new RemoveDeadEndsStep(),
            new AddBridgesStep(),
            new FindCityBlocksStep(),
            new CreateBuildingPlotsStep(),
            new AddSidewalksStep(),
            new AddRoadMeshesStep(),
            new AddClickColliderStep()
        };
    
    public GenerationData Generate(GenerationOptions options)
    {
        var data = new GenerationData(); 
        foreach (var step in _steps)
        {
            step.RunStep(options, data);
        }
        
        return data;
    }
}
