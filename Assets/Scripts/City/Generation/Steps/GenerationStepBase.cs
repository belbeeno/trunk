public abstract class GenerationStepBase : IGenerationStep
{
    protected GenerationOptions options;
    protected GenerationData data;
        
    public GenerationData RunStep(GenerationOptions options, GenerationData data)
    {
        this.options = options;
        this.data = data;
        return Run();
    }
        
    public abstract GenerationData Run();
}