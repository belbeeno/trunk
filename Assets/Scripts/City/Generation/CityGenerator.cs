using System;
using System.Collections.Generic;

public class CityGenerator
{    
    public GenerationResult Generate(GenerationOptions options)
    {
        var steps = new IGenerationStep[] {
            new GenerateGridStep(),
            new AddRiverStep()
        };
        
        var data = new GenerationData(); 
        foreach (var step in steps)
        {
            data = step.Run(options, data);
        }
        var result = data.ToGenerationResult();
            
        return result;
    }
}