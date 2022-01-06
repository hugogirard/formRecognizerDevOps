

namespace DemoForm;

public class ModelInfo
{
    public string ModelId { get; }

    public string Description { get; }

    public DateTimeOffset CreatedOn { get; set; }

    public ModelInfo(DocumentModelInfo documentModelInfo)
    {
        ModelId = documentModelInfo.ModelId;
        Description = documentModelInfo.Description;
        CreatedOn = documentModelInfo.CreatedOn;
    }
}
