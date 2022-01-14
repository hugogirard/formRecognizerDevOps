namespace Demo.Shared.Models;

public class CopyModelParameter
{
    public string SourceModelId { get; set; }

    public MODEL_ENVIRONMENT SourceEnvironment { get; set; }

    public MODEL_ENVIRONMENT DestinationEnvironment { get; set; }

}
