

using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace DemoForm;

public class ValidateModelExists
{
    private readonly ILogger<ValidateModelExists> _logger;
    private readonly IFormClientFactory _formFactory;

    public ValidateModelExists(ILogger<ValidateModelExists> log, IFormClientFactory factory)
    {
        _logger = log;
        _formFactory = factory;
    }

    [FunctionName("ValidateModelExists")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "modelId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **modelId** to validate")]
    [OpenApiParameter(name: "environment", In = ParameterLocation.Query, Required = true, Type = typeof(FORM_RECOGNIZER_ENVIRONMENT), Description = "The **environment** where is the model located")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DocumentModel), Description = "The Custom Model definition")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {
        try
        {
            string modelId = req.Query["modelid"];
            string environment = req.Query["environment"];

            if (string.IsNullOrEmpty(modelId)) 
            {
                return new BadRequestObjectResult("The modelId is required");
            }

            FORM_RECOGNIZER_ENVIRONMENT formEnvironment;
            if (!Enum.TryParse(environment,out formEnvironment))
            {
                return new BadRequestObjectResult("The environment is invalid");
            }

            var trainingClient = _formFactory.CreateAdministrationClient(formEnvironment);

            Response<DocumentModel> operation = await trainingClient.GetModelAsync(modelId);
                        
            return new OkObjectResult(operation.Value);
      
        }        
        catch (RequestFailedException rex) 
        { 
            if (rex.Status == 404)
                return new NotFoundResult();

            _logger.LogError(rex.Message,rex);
   
            return new ObjectResult("Something happen when retrieving the model") { StatusCode = rex.Status };
        }
        catch (Exception ex)
        {            
            _logger.LogError(ex.Message, ex);
            return new ObjectResult("Internal Server Error") { StatusCode = 500 };
        }
    }
}

