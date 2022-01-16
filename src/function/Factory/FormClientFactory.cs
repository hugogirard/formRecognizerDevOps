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