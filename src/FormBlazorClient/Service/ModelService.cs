using System.Dynamic;
using System.Text.Json;

namespace FormBlazorClient.Service;

public class ModelService : IModelService
{
    private readonly IHttpClientFactory _httpClientfactory;
    private readonly string BASE_URL;
    private readonly string FUNCTION_CODE;

    public ModelService(IHttpClientFactory httpClientfactory, IConfiguration configuration)
    {
        _httpClientfactory = httpClientfactory;
        BASE_URL = configuration["FunctionBaseUrl"];
        FUNCTION_CODE = configuration["FunctionKeyCode"];
    }

    public async Task<IEnumerable<ModelDefinition>> GetModelsAsync(ModelEnvironment environment)
    {
        string url = $"{BASE_URL}/GetModel?environment={environment}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("x-functions-key", FUNCTION_CODE);

        var client = _httpClientfactory.CreateClient();

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<IEnumerable<ModelDefinition>>(json);

        }

        return new List<ModelDefinition>();
    }
    public async Task<bool> DeleteModelAsync(string modelId, ModelEnvironment environment)
    {
        string url = $"{BASE_URL}/DeleteModel";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("x-functions-key", FUNCTION_CODE);

        dynamic payload = new ExpandoObject();
        payload.modelId = modelId;
        payload.environment = environment;

        request.Content = new StringContent(JsonSerializer.Serialize(payload));
        var client = _httpClientfactory.CreateClient();

        var response = await client.DeleteAsync(url);

        if (response.IsSuccessStatusCode)
            return true;

        return false;
    }


}
