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
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DocumentModel), Description = "The Custom Model definition")]
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

            var buildOptions = new BuildModelOptions();
            if (!string.IsNullOrEmpty(modelDescription)) 
            {
                buildOptions.ModelDescription = modelDescription;
            }

            BuildModelOperation operation;
            if (!string.IsNullOrEmpty(modelId))
            {
                operation = await trainingClient.StartBuildModelAsync(sas, modelId: modelId, buildOptions);
            }
            else 
            {
                operation = await trainingClient.StartBuildModelAsync(sas,buildModelOptions: buildOptions);
            }

            Response<DocumentModel> operationResponse = await operation.WaitForCompletionAsync();

            // To check response here

            DocumentModel documentModel = operationResponse.Value;

            return new OkObjectResult(documentModel);       
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ObjectResult("Internal Server Error") { StatusCode = 500 };
        }
    }
}

