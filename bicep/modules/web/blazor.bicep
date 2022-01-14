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
