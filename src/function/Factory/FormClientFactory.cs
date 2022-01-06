using System.Collections.Generic;

namespace DemoForm;

public class FormClientFactory : IFormClientFactory
{
    private readonly Dictionary<FORM_RECOGNIZER_ENVIRONMENT, DocumentModelAdministrationClient> _clients;

    public FormClientFactory()
    {
        _clients = new Dictionary<FORM_RECOGNIZER_ENVIRONMENT, DocumentModelAdministrationClient>();
    }

    public DocumentModelAdministrationClient CreateClient(FORM_RECOGNIZER_ENVIRONMENT env)
    {
        if (_clients.ContainsKey(env))
        {
            return _clients[env];
        }

        string formRecognizerEndpoint;
        string formRecognizerKey;

        switch (env)
        {
            case FORM_RECOGNIZER_ENVIRONMENT.DEV:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerDevEndpoint");
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerDevKey");
                break;
            case FORM_RECOGNIZER_ENVIRONMENT.QA:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerQaEndpoint");
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerQaKey");
                break;
            default:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerProdEndpoint");
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerProdKey");
                break;
        }

        var credential = new AzureKeyCredential(formRecognizerKey);
        var trainingClient = new DocumentModelAdministrationClient(new Uri(formRecognizerEndpoint), credential);

        _clients.Add(env, trainingClient);

        return trainingClient;
    }

}