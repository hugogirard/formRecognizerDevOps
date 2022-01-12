using System.Collections.Generic;

namespace DemoForm;

public class Document
{
    private readonly ILogger<Document> _logger;
    private readonly IFormClientFactory _formClientFactory;

    public Document(ILogger<Document> log, IFormClientFactory formClientFactory)
    {
        _logger = log;
        _formClientFactory = formClientFactory;
    }



    [FunctionName("AnalyzeDocument")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody("application/json", typeof(DocumentInfo), Description = "The info of the document to analyze", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<DocumentResult>), Description = "The results of the form analyzer extraction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        DocumentInfo documentInfo;
        if (req.Body != null)
        {
            using (var sr = new StreamReader(req.Body))
            {
                string requestBody = await sr.ReadToEndAsync();
                documentInfo = JsonConvert.DeserializeObject<DocumentInfo>(requestBody);

                var validator = new DocumentInfoParameterValidator();
                var result = validator.Validate(documentInfo);

                if (!result.IsValid)
                    return new BadRequestObjectResult("The parameters are invalid");

                var client = _formClientFactory.CreateAnalysisClient(documentInfo.Environment);

                AnalyzeDocumentOperation operation = await client.StartAnalyzeDocumentFromUriAsync(documentInfo.ModelId, 
                                                                                                   new Uri(documentInfo.DocumentUrl));
                // This should not be done in PRODUCTION
                // Should query the result at a X frequency, this code is only for demo purpose
                await operation.WaitForCompletionAsync();

                AnalyzeResult operationResult = operation.Value;
                var documentsResult = new List<DocumentResult>();

                foreach (AnalyzedDocument document in operationResult.Documents)
                {
                    var documentResult = new DocumentResult 
                    {
                        DocType = document.DocType
                    };
                    
                    foreach (KeyValuePair<string, DocumentField> fieldKvp in document.Fields)
                    {
                        string fieldName = fieldKvp.Key;
                        DocumentField field = fieldKvp.Value;

                        documentResult.Fields.Add(new Fields 
                        { 
                            FieldName = fieldName,
                            Content = field.Content,
                            Confidence = field.Confidence
                        });
                    }

                    documentsResult.Add(documentResult);
                }

                return new OkObjectResult(documentsResult);
            }
        }

        return new BadRequestObjectResult("The parameters are invalid");
    }
}

