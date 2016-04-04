public interface IGenerationStep
{
    GenerationData Run(GenerationOptions options, GenerationData data);
}