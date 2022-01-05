using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure.AI.FormRecognizer.Training;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace DemoForm;

public class TrainModel
{
    private readonly ILogger<TrainModel> _logger;
    private readonly IFormClientFactory _formClientFactory;
    private readonly BlobContainerClient _containerClient;

    public TrainModel(ILogger<TrainModel> log, IFormClientFactory formClientFactory, BlobContainerClient blobContainerClient)
    {
        _logger = log;
        _formClientFactory = formClientFactory;
        _containerClient = blobContainerClient;
    }

    [FunctionName("TrainModel")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "modelName", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The name of the model")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CustomFormModel), Description = "The Custom Model definition")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        try
        {
            string modelName = req.Query["modelName"];

            if (string.IsNullOrEmpty(modelName)) 
            {
                return new BadRequestObjectResult("The query string modelName need to be present");
            }             

            var sas = _containerClient.GenerateSasUri(Azure.Storage.Sas.BlobContainerSasPermissions.All,
                                                      DateTime.UtcNow.AddMinutes(15));
            
            // By default the training is done in the DEV environment
            var trainingClient = _formClientFactory.CreateClient(ENVIRONMENT.DEV);

            var response = await trainingClient.StartTrainingAsync(sas, useTrainingLabels: true, modelName);
            
            CustomFormModel model = await response.WaitForCompletionAsync();     

            return new OkObjectResult(model);       
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ObjectResult("Internal Server Error") { StatusCode = 500 };
        }
    }
}

