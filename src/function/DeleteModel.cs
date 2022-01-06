namespace DemoForm;

public class DeleteModel
{
    private readonly ILogger<DeleteModel> _logger;
    private readonly IFormClientFactory _factory;

    public DeleteModel(ILogger<DeleteModel> log, IFormClientFactory factory)
    {
        _logger = log;
        _factory = factory;
    }

    [FunctionName("DeleteModel")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody("application/json", typeof(DeleteModelParameter), Description = "The Delete Parameter", Required = false)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)] HttpRequest req)
    {


        try
        {
            if (req.Body == null)
                return new BadRequestObjectResult("The DeleteModel parameters cannot be null");

            DeleteModelParameter deleteModelParameter;
            using (var sr = new StreamReader(req.Body))
            {
                string requestBody = await sr.ReadToEndAsync();
                deleteModelParameter = JsonConvert.DeserializeObject<DeleteModelParameter>(requestBody);
            }

            var validator = new DeleteModelParameterValidator();
            var result = validator.Validate(deleteModelParameter);

            if (!result.IsValid)
                return new BadRequestObjectResult("The DeleteModel parameters is invalid");

            var trainingClient = _factory.CreateClient(deleteModelParameter.Environment);

            Response response = await trainingClient.DeleteModelAsync(deleteModelParameter.ModelId);
            
            return new ObjectResult(string.Empty) { StatusCode = response.Status };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ObjectResult("Internal Server Error") { StatusCode = 500 };
        }
    }
}

