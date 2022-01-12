using System.Collections.Generic;

namespace DemoForm;

public class GetModel
{
    private readonly ILogger<GetModel> _logger;
    private readonly IFormClientFactory _formClientFactory;

    public GetModel(ILogger<GetModel> log, IFormClientFactory factory)
    {
        _logger = log;
        _formClientFactory = factory;
    }

    [FunctionName("GetModel")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "environment", In = ParameterLocation.Query, Required = true, Type = typeof(FORM_RECOGNIZER_ENVIRONMENT), Description = "The environment to get the models")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ModelInfo>), Description = "The models in the environment")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {

        try
        {
            string environment = req.Query["environment"];

            if (string.IsNullOrEmpty(environment))
                return new BadRequestObjectResult("The Environment query string cannot be null");

            FORM_RECOGNIZER_ENVIRONMENT formEnvironment;
            if (!Enum.TryParse(environment, out formEnvironment))
                return new BadRequestObjectResult("The Environment query string is invalid");

            var trainingClient = _formClientFactory.CreateAdministrationClient(formEnvironment);

            AsyncPageable<DocumentModelInfo> models = trainingClient.GetModelsAsync();
            var modelsInfo = new List<ModelInfo>();
            await foreach (DocumentModelInfo modelInfo in models)
            {
                // Remove the pre-build model from the list
                if (!modelInfo.ModelId.ToLower().Contains("prebuilt"))
                    modelsInfo.Add(new ModelInfo(modelInfo));
            }

            return new OkObjectResult(modelsInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ObjectResult("Internal Server Error") { StatusCode = 500 };
        }


    }
}

