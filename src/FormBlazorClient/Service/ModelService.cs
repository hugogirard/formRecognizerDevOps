/*
* Notice: Any links, references, or attachments that contain sample scripts, code, or commands comes with the following notification.
*
* This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.
* THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
*
* We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object code form of the Sample Code,
* provided that You agree:
*
* (i) to not use Our name, logo, or trademarks to market Your software product in which the Sample Code is embedded;
* (ii) to include a valid copyright notice on Your software product in which the Sample Code is embedded; and
* (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims or lawsuits,
* including attorneys’ fees, that arise or result from the use or distribution of the Sample Code.
*
* Please note: None of the conditions outlined in the disclaimer above will superseded the terms and conditions contained within the Premier Customer Services Description.
*
* DEMO POC - "AS IS"
*/
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

    public async Task<IEnumerable<DocumentResult>> AnalyzeDocumentAsync(string documentUrl,string modelId, MODEL_ENVIRONMENT environment)
    {

        string url = $"{BASE_URL}/AnalyzeDocument";

        dynamic payload = new ExpandoObject();
        payload.ModelId = modelId;
        payload.Environment = environment;
        payload.DocumentUrl = documentUrl;

        HttpResponseMessage response = await SendRequestAsync(url,HttpMethod.Post,payload);

        if (!response.IsSuccessStatusCode)
            return null;

        string json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<IEnumerable<DocumentResult>>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

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
