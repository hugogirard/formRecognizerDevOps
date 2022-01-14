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

    public async Task<IList<ModelInfo>> GetModelsAsync(MODEL_ENVIRONMENT environment)
    {
        string url = $"{BASE_URL}/GetModel?environment={environment}";

        HttpResponseMessage response = await SendRequestAsync(url, HttpMethod.Get);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<IList<ModelInfo>>(json,new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        }

        return new List<ModelInfo>();
    }
    public async Task<bool> DeleteModelAsync(string modelId, MODEL_ENVIRONMENT environment)
    {
        string url = $"{BASE_URL}/DeleteModel";
             
        dynamic payload = new ExpandoObject();
        payload.modelId = modelId;
        payload.environment = environment;

        HttpResponseMessage response = await SendRequestAsync(url, HttpMethod.Delete, payload);

        if (response.IsSuccessStatusCode)
            return true;

        return false;
    }

    public async Task AnalyzeDocumentAsync(string documentUrl,string modelId, MODEL_ENVIRONMENT environment)
    {

        string url = $"{BASE_URL}/AnalyzeDocument";

        
    }

    private async Task<HttpResponseMessage> SendRequestAsync(string url, HttpMethod method) 
    {
        return await SendRequestAsync(url, method, null);
    }

    private async Task<HttpResponseMessage> SendRequestAsync(string url, HttpMethod method, dynamic? payload) 
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("x-functions-key", FUNCTION_CODE);
        if (payload != null) 
        {
            request.Content = new StringContent(JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

        var client = _httpClientfactory.CreateClient();
        return await client.SendAsync(request);
    }


}
