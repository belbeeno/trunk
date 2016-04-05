using System;
using System.Collections.Generic;
using System.Linq;

public class CityGenerator
{    
    public GenerationResult Generate(GenerationOptions options)
    {
        var steps = new IGenerationStep[] {
            new GenerateGridStep(),
            new AddRiverStep(),
            new ClearAreaNearRiverStep(),
            new AddBridgesStep()
        };
        
        var initialData = new GenerationData(options); 
        var finalData = steps.Aggregate(initialData, (data, step) => step.RunStep(options, data));
        var result = finalData.ToGenerationResult();
            
        return result;
    }
}