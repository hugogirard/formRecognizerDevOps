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
    [OpenApiParameter(name: "environment", In = ParameterLocation.Query, Required = true, Type = typeof(MODEL_ENVIRONMENT), Description = "The **environment** where is the model located")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DocumentModelDetails), Description = "The Custom Model definition")]
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

            MODEL_ENVIRONMENT formEnvironment;
            if (!Enum.TryParse(environment,out formEnvironment))
            {
                return new BadRequestObjectResult("The environment is invalid");
            }

            var trainingClient = _formFactory.CreateAdministrationClient(formEnvironment);

            Response<DocumentModelDetails> operation = await trainingClient.GetDocumentModelAsync(modelId);
                        
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

