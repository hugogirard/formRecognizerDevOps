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
    [OpenApiRequestBody("application/json",typeof(Model),Description = "The model parameter", Required = false)]   
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DocumentModelDetails), Description = "The Custom Model definition")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        try
        {
            string modelId = string.Empty;
            string modelDescription = string.Empty;
            if (req.Body != null) 
            {
                using (var sr = new StreamReader(req.Body))
                {
                    string requestBody = await sr.ReadToEndAsync();
                    var model = JsonConvert.DeserializeObject<Model>(requestBody);
                    modelId = model?.ModelId ?? string.Empty;
                    modelDescription = model?.Description ?? string.Empty;
                }
            }


            var sas = _containerClient.GenerateSasUri(Azure.Storage.Sas.BlobContainerSasPermissions.All,
                                                      DateTime.UtcNow.AddMinutes(15));
            
            // By default the training is done in the DEV environment
            var trainingClient = _formClientFactory.CreateAdministrationClient(MODEL_ENVIRONMENT.DEV);

            var buildOptions = new BuildDocumentModelOptions();
            if (!string.IsNullOrEmpty(modelDescription)) 
            {
                buildOptions.Description = modelDescription;
            }
                        
            BuildDocumentModelOperation operation;
            if (!string.IsNullOrEmpty(modelId))
            {
                operation = await trainingClient.BuildDocumentModelAsync(WaitUntil.Completed, sas, DocumentBuildMode.Template, modelId, options: buildOptions);
            }
            else 
            {
                operation = await trainingClient.BuildDocumentModelAsync(WaitUntil.Completed, sas, DocumentBuildMode.Template, options: buildOptions);
            }

            //Response<DocumentModel> operationResponse = await operation.WaitForCompletionAsync();
            // To check response here            

            DocumentModelDetails documentModel = operation.Value;

            return new OkObjectResult(documentModel);       
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ObjectResult("Internal Server Error") { StatusCode = 500 };
        }
    }
}

