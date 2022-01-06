namespace DemoForm;

public class Model
{
    [JsonProperty("modelId")]
    public string ModelId { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
}
