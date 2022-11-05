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
                
                AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed,
                                                                                              documentInfo.ModelId, 
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
                        DocType = document.DocumentType
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

