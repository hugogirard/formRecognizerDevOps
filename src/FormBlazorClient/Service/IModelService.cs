
namespace FormBlazorClient.Service
{
    public interface IModelService
    {
        Task<bool> DeleteModelAsync(string modelId, MODEL_ENVIRONMENT environment);
        Task<IList<ModelInfo>> GetModelsAsync(MODEL_ENVIRONMENT environment);

        Task<IEnumerable<DocumentResult>> AnalyzeDocumentAsync(string documentUrl, string modelId, MODEL_ENVIRONMENT environment);
    }
}