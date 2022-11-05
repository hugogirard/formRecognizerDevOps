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

            var trainingClient = _factory.CreateAdministrationClient(deleteModelParameter.Environment);
            
            Response response = await trainingClient.DeleteDocumentModelAsync(deleteModelParameter.ModelId);
            
            return new ObjectResult(string.Empty) { StatusCode = response.Status };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ObjectResult("Internal Server Error") { StatusCode = 500 };
        }
    }
}

