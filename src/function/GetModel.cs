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
    [OpenApiParameter(name: "environment", In = ParameterLocation.Query, Required = true, Type = typeof(MODEL_ENVIRONMENT), Description = "The environment to get the models")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ModelInfo>), Description = "The models in the environment")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {

        try
        {
            string environment = req.Query["environment"];

            if (string.IsNullOrEmpty(environment))
                return new BadRequestObjectResult("The Environment query string cannot be null");

            MODEL_ENVIRONMENT formEnvironment;
            if (!Enum.TryParse(environment, out formEnvironment))
                return new BadRequestObjectResult("The Environment query string is invalid");

            var trainingClient = _formClientFactory.CreateAdministrationClient(formEnvironment);
            
            AsyncPageable<DocumentModelSummary> models = trainingClient.GetDocumentModelsAsync();
            var modelsInfo = new List<ModelInfo>();
            await foreach (DocumentModelSummary modelInfo in models)
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

