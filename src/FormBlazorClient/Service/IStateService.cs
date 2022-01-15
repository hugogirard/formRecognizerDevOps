
namespace FormBlazorClient.Service
{
    public interface IStateService
    {
        MODEL_ENVIRONMENT SelectedEnvironment { get; set; }
        ModelInfo SelectedModel { get; set; }
    }
}