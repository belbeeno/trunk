public interface IGenerationStep
{
    GenerationData RunStep(GenerationOptions options, GenerationData data);
}