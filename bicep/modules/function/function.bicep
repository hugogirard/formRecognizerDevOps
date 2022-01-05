param location string
param suffix string

param appInsightKey string
param appInsightCnxString string

param strAccountName string
param strAccountId string
param strAccountApiVersion string

param formRecognizerDevEndpoint string
param formRecognizerQAEndpoint string
param formRecognizerProdEndpoint string

param formRecognizerDevKey string
param formRecognizerQAKey string
param formRecognizerProdKey string

var appServiceName = 'func-plan-${suffix}'
var functionAppName = 'func-form-${suffix}'

resource serverFarm 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: appServiceName
  location: location
  sku: {
    tier: 'Dynamic'
    name: 'Y1'
  }
}

resource function 'Microsoft.Web/sites@2020-06-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: serverFarm.id    
    siteConfig: {
      netFrameworkVersion: 'v6.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsightKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightCnxString
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${strAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(strAccountId, strAccountApiVersion).keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${strAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(strAccountId, strAccountApiVersion).keys[0].value}'
        }
        {
          name: 'ModelContainer'
          value: 'model'
        }
        {
          name: 'FormRecognizerDevEndpoint'
          value: formRecognizerDevEndpoint
        }
        {
          name: 'FormRecognizerDevKey'
          value: formRecognizerDevKey
        }
        {
          name: 'FormRecognizerQaEndpoint'
          value: formRecognizerQAEndpoint
        }
        {
          name: 'FormRecognizerQaKey'
          value: formRecognizerQAKey
        }
        {
          name: 'FormRecognizerProdEndpoint'
          value: formRecognizerProdEndpoint
        }
        {
          name: 'FormRecognizerProdKey'
          value: formRecognizerProdKey
        }                
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: 'processorapp092'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'WEBSITE_NODE_DEFAULT_VERSION'
          value: '~12'
        }
      ]
    }
  }
}

output functionName string = function.name
