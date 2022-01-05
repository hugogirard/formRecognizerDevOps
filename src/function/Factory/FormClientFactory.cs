
using System;
using System.Collections.Generic;
using Azure;
using Azure.AI.FormRecognizer.Training;

namespace DemoForm;

public class FormClientFactory : IFormClientFactory
{
    private readonly Dictionary<ENVIRONMENT, FormTrainingClient> _clients;

    public FormClientFactory()
    {
        _clients = new Dictionary<ENVIRONMENT, FormTrainingClient>();
    }

    public FormTrainingClient CreateClient(ENVIRONMENT env)
    {
        if (_clients.ContainsKey(env))
        {
            return _clients[env];
        }

        string formRecognizerEndpoint;
        string formRecognizerKey;

        switch (env)
        {
            case ENVIRONMENT.DEV:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerDevEndpoint");
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerDevKey");
                break;
            case ENVIRONMENT.QA:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerQaEndpoint");
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerQaKey");
                break;
            default:
                formRecognizerEndpoint = Environment.GetEnvironmentVariable("FormRecognizerProdEndpoint");
                formRecognizerKey = Environment.GetEnvironmentVariable("FormRecognizerProdKey");
                break;
        }

        var credential = new AzureKeyCredential(formRecognizerKey);
        var trainingClient = new FormTrainingClient(new Uri(formRecognizerEndpoint), credential);

        _clients.Add(env, trainingClient);

        return trainingClient;
    }

}