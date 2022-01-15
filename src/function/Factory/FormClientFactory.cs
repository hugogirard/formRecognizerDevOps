using System.Collections.Generic;

namespace DemoForm;

public class FormClientFactory : IFormClientFactory
{
    private readonly Dictionary<MODEL_ENVIRONMENT, DocumentModelAdministrationClient> _adminClients;
    private readonly Dictionary<MODEL_ENVIRONMENT, DocumentAnalysisClient> _analysisClients;

    public FormClientFactory()
    {
        _adminClients = new Dictionary<MODEL_ENVIRONMENT, DocumentModelAdministrationClient>();
        _analysisClients = new Dictionary<MODEL_ENVIRONMENT, DocumentAnalysisClient>();
    }

    public DocumentModelAdministrationClient CreateAdministrationClient(MODEL_ENVIRONMENT env)
    {
        if (_adminClients.ContainsKey(env))
        {
            return _adminClients[env];
        }

        var credential = GetAzureCredentials(env);
        var formRecognizerEndpoint = GetFormRecognizerEndpoint(env);

        var trainingClient = new DocumentModelAdministrationClient(new Uri(formRecognizerEndpoint), credential);

        _adminClients.Add(env, trainingClient);

        return trainingClient;
    }

    public DocumentAnalysisClient CreateAnalysisClient(MODEL_ENVIRONMENT env)
    {
        if (_analysisClients.ContainsKey(env))
        {
            return _analysisClients[env];
        }

        var credential = GetAzureCredentials(env);
        var formRecognizerEndpoint = GetFormRecognizerEndpoint(env);

        var client = new DocumentAnalysisClient(new Uri(formRecognizerEndpoint), credential);

        _analysisClients.Add(env, client);

        return client;
    }

    private AzureKeyCredential GetAzureCredentials(MODEL_ENVIRONMENT env) 
    {        
        string formRecognizerKey;

        switch (env)
        {
            case MODEL_ENVIRONMENT.DEV:                
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerDevKey");
                break;
            case MODEL_ENVIRONMENT.QA:     
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerQaKey");
                break;
            default:                
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerProdKey");
                break;
        }

        return new AzureKeyCredential(formRecognizerKey);
    }

    private string GetFormRecognizerEndpoint(MODEL_ENVIRONMENT env) 
    {
        string formRecognizerEndpoint;
        
        switch (env)
        {
            case MODEL_ENVIRONMENT.DEV:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerDevEndpoint");        
                break;
            case MODEL_ENVIRONMENT.QA:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerQaEndpoint");                
                break;
            default:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerProdEndpoint");                
                break;
        }

        return formRecognizerEndpoint;
    }
}