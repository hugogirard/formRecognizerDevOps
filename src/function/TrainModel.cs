using System.IO;
using System.Linq;
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
    [OpenApiParameter(name: "modelId",Required = true, Description = "The name of the modelId")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DocumentModel), Description = "The Custom Model definition")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        try
        {

            // Validate parameters here
            string modelName = req.Query["modelId"];
            if (string.IsNullOrEmpty(modelName)) 
            {
                return new BadRequestObjectResult("The modelId parameter cannot be null");
            }

            var sas = _containerClient.GenerateSasUri(Azure.Storage.Sas.BlobContainerSasPermissions.All,
                                                      DateTime.UtcNow.AddMinutes(15));
            
            // By default the training is done in the DEV environment
            var trainingClient = _formClientFactory.CreateClient(ENVIRONMENT.DEV);

            BuildModelOperation operation = await trainingClient.StartBuildModelAsync(sas);
            Response<DocumentModel> operationResponse = await operation.WaitForCompletionAsync();

            // To check response here

            DocumentModel model = operationResponse.Value;

            return new OkObjectResult(model);       
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ObjectResult("Internal Server Error") { StatusCode = 500 };
        }
    }
}

