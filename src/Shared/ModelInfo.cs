
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace Demo.Shared.Models;

public class ModelInfo
{
    public string ModelId { get; set; }

    public string Description { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public ModelInfo()
    {

    }

    public ModelInfo(DocumentModelInfo documentModelInfo)
    {
        ModelId = documentModelInfo.ModelId;
        Description = documentModelInfo.Description;
        CreatedOn = documentModelInfo.CreatedOn;
    }
}
