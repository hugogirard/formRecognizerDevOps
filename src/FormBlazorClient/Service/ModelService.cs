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

    public async Task<IList<ModelDefinition>> GetModelsAsync(ModelEnvironment environment)
    {
        string url = $"{BASE_URL}/GetModel?environment={environment}";

        HttpResponseMessage response = await SendRequestAsync(url, HttpMethod.Get);

        //var request = new HttpRequestMessage(HttpMethod.Get, url);
        //request.Headers.Add("x-functions-key", FUNCTION_CODE);

        //var client = _httpClientfactory.CreateClient();

        //var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<IList<ModelDefinition>>(json,new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        }

        return new List<ModelDefinition>();
    }
    public async Task<bool> DeleteModelAsync(string modelId, ModelEnvironment environment)
    {
        string url = $"{BASE_URL}/DeleteModel";
        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Add("x-functions-key", FUNCTION_CODE);

        dynamic payload = new ExpandoObject();
        payload.modelId = modelId;
        payload.environment = environment;

        request.Content = new StringContent(JsonSerializer.Serialize(payload,new JsonSerializerOptions 
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
        var client = _httpClientfactory.CreateClient();

        var response = await client.DeleteAsync(url);

        if (response.IsSuccessStatusCode)
            return true;

        return false;
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
