
namespace FormBlazorClient.Service
{
    public interface IModelService
    {
        Task<bool> DeleteModelAsync(string modelId, MODEL_ENVIRONMENT environment);
        Task<IList<ModelInfo>> GetModelsAsync(MODEL_ENVIRONMENT environment);
    }
}