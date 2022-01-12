using System.Collections.Generic;

namespace DemoForm;

public class FormClientFactory : IFormClientFactory
{
    private readonly Dictionary<FORM_RECOGNIZER_ENVIRONMENT, DocumentModelAdministrationClient> _adminClients;
    private readonly Dictionary<FORM_RECOGNIZER_ENVIRONMENT, DocumentAnalysisClient> _analysisClients;

    public FormClientFactory()
    {
        _adminClients = new Dictionary<FORM_RECOGNIZER_ENVIRONMENT, DocumentModelAdministrationClient>();
        _analysisClients = new Dictionary<FORM_RECOGNIZER_ENVIRONMENT, DocumentAnalysisClient>();
    }

    public DocumentModelAdministrationClient CreateAdministrationClient(FORM_RECOGNIZER_ENVIRONMENT env)
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

    public DocumentAnalysisClient CreateAnalysisClient(FORM_RECOGNIZER_ENVIRONMENT env)
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

    private AzureKeyCredential GetAzureCredentials(FORM_RECOGNIZER_ENVIRONMENT env) 
    {        
        string formRecognizerKey;

        switch (env)
        {
            case FORM_RECOGNIZER_ENVIRONMENT.DEV:                
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerDevKey");
                break;
            case FORM_RECOGNIZER_ENVIRONMENT.QA:     
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerQaKey");
                break;
            default:                
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerProdKey");
                break;
        }

        return new AzureKeyCredential(formRecognizerKey);
    }

    private string GetFormRecognizerEndpoint(FORM_RECOGNIZER_ENVIRONMENT env) 
    {
        string formRecognizerEndpoint;
        
        switch (env)
        {
            case FORM_RECOGNIZER_ENVIRONMENT.DEV:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerDevEndpoint");        
                break;
            case FORM_RECOGNIZER_ENVIRONMENT.QA:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerQaEndpoint");                
                break;
            default:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerProdEndpoint");                
                break;
        }

        return formRecognizerEndpoint;
    }
}