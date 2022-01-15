namespace FormBlazorClient.Service;

public class StateService : IStateService
{
    public ModelInfo SelectedModel { get; set; }

    public MODEL_ENVIRONMENT SelectedEnvironment { get; set; }
}
