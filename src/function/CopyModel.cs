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
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DocumentModelDetails), Description = "The Copy Custom Model definition")]
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
                var response = await sourceClient.GetDocumentModelAsync(copyModel.SourceModelId);

                if (!response.GetRawResponse().Status.IsSuccessStatusCode())
                    return new NotFoundObjectResult("Cannot get the info of the source model");
                
                DocumentModelCopyAuthorization copyAuthorization = await targetClient.GetCopyAuthorizationAsync(response.Value.ModelId,response.Value.Description);
                CopyDocumentModelToOperation newModelOperation = await sourceClient.CopyDocumentModelToAsync(WaitUntil.Completed, copyModel.SourceModelId, copyAuthorization);                
                
                return new OkObjectResult(newModelOperation.Value);

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

