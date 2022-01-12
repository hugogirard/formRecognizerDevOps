
namespace FormBlazorClient.Service
{
    public interface IModelService
    {
        Task<bool> DeleteModelAsync(string modelId, ModelEnvironment environment);
        Task<IEnumerable<ModelDefinition>> GetModelsAsync(ModelEnvironment environment);
    }
}