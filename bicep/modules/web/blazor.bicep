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
param storageName string
param appInsightName string
param functionHostname string

var appPlanName = 'plan-blazor-${suffix}'
var webAppName = 'blazor-admin-${suffix}'

resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: appPlanName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightName
  scope: resourceGroup(resourceGroup().name)
}

resource str 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: storageName
  scope: resourceGroup(resourceGroup().name)
}

resource appService 'Microsoft.Web/sites@2021-02-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    clientAffinityEnabled: false
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'UploadStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${str.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(str.id, str.apiVersion).keys[0].value}'
        }
        {
          name: 'Container'
          value: 'upload'
        }
        {
          name: 'FunctionBaseUrl'
          value: functionHostname
        }        
      ]
      metadata: [
        {
           name: 'CURRENT_STACK'
           value: 'dotnetcore'
        }
      ]
      alwaysOn: true
    }
  }
}

output webName string = appService.name
