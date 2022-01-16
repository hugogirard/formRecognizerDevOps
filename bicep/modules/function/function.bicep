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
param location string
param suffix string

param appInsightKey string
param appInsightCnxString string

param strAccountName string
param strAccountId string
param strAccountApiVersion string

param devStorageName string
param devFormRecognizerName string
param qaFormRecognizerName string
param prodFormRecognizerName string

param devResourceGroupName string
param qaResourceGroupName string
param prodResourceGroupName string

var appServiceName = 'func-plan-${suffix}'
var functionAppName = 'func-form-${suffix}'

resource devStorage 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: devStorageName
  scope: resourceGroup(devResourceGroupName)
}

resource formDev 'Microsoft.CognitiveServices/accounts@2021-04-30' existing = {
  name: devFormRecognizerName
  scope: resourceGroup(devResourceGroupName)
}

resource formQA 'Microsoft.CognitiveServices/accounts@2021-04-30' existing = {
  name: qaFormRecognizerName
  scope: resourceGroup(qaResourceGroupName)
}

resource formPROD 'Microsoft.CognitiveServices/accounts@2021-04-30' existing = {
  name: prodFormRecognizerName
  scope: resourceGroup(prodResourceGroupName)
}

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
          value: formDev.properties.endpoint
        }                  
        {
          name: 'FormRecognizerDevKey'
          value: listKeys(formDev.id,formDev.apiVersion).key1
        }        
        {
          name: 'FormRecognizerQaEndpoint'
          value: formQA.properties.endpoint
        }
        {
          name: 'FormRecognizerQaKey'
          value: listKeys(formQA.id,formQA.apiVersion).key1
        }        
        {
          name: 'FormRecognizerProdEndpoint'
          value: formPROD.properties.endpoint
        }                 
        {
          name: 'FormRecognizerProdKey'
          value: listKeys(formPROD.id,formPROD.apiVersion).key1
        } 
        {
          name: 'DevStorageCnxString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${devStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(devStorage.id, devStorage.apiVersion).keys[0].value}'
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

output functionName string = functionAppName
output functionHostname string = 'https://${function.properties.hostNames[0]}/api'
