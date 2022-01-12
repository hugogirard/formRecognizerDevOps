
namespace FormBlazorClient.Service
{
    public interface IModelService
    {
        Task<bool> DeleteModelAsync(string modelId, ModelEnvironment environment);
        Task<IList<ModelDefinition>> GetModelsAsync(ModelEnvironment environment);
    }
}