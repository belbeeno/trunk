public abstract class GenerationStepBase : IGenerationStep
{
    protected GenerationOptions options;
    protected GenerationData data;
        
    public void RunStep(GenerationOptions options, GenerationData data)
    {
        this.options = options;
        this.data = data;
        
        Run();
    }
        
    public abstract void Run();
}