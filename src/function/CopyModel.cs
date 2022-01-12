namespace DemoForm;

public class CopyModel
{
    private readonly ILogger<CopyModel> _logger;
    private readonly IFormClientFactory _formClientFactory;

    public CopyModel(ILogger<CopyModel> log, IFormClientFactory formClientFactory)
    {
        _logger = log;
        _formClientFactory = formClientFactory;
    }

    [FunctionName("CopyModel")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody("application/json", typeof(CopyModelParameter), Description = "The Copy Model Information", Required = false)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DocumentModel), Description = "The Copy Custom Model definition")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {

        try
        {
            CopyModelParameter copyModel;
            if (req.Body != null)
            {
                using (var sr = new StreamReader(req.Body))
                {
                    string requestBody = await sr.ReadToEndAsync();
                    copyModel = JsonConvert.DeserializeObject<CopyModelParameter>(requestBody);
                }

                var validator = new CopyModelParameterValidator();
                var result = validator.Validate(copyModel);

                if (!result.IsValid)
                    return new BadRequestObjectResult("The parameters are invalid");

                var sourceClient = _formClientFactory.CreateAdministrationClient(copyModel.SourceEnvironment);
                var targetClient = _formClientFactory.CreateAdministrationClient(copyModel.DestinationEnvironment);

                // Get info of the model to copy (description)
                var response = await sourceClient.GetModelAsync(copyModel.SourceModelId);

                if (!response.GetRawResponse().Status.IsSuccessStatusCode())
                    return new NotFoundObjectResult("Cannot get the info of the source model");

                CopyAuthorization copyAuthorization = await targetClient.GetCopyAuthorizationAsync(response.Value.ModelId,response.Value.Description);

                CopyModelOperation newModelOperation = await sourceClient.StartCopyModelAsync(copyModel.SourceModelId, copyAuthorization);

                await newModelOperation.WaitForCompletionAsync();
                DocumentModel newModel = newModelOperation.Value;

                return new OkObjectResult(newModel);

            }
            else
            {
                return new BadRequestObjectResult("The CopyModel parameters cannot be null");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ObjectResult("Internal Server Error") { StatusCode = 500 };
        }
    }    
}

