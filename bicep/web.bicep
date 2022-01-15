param location string
param strName string
param appInsightName string
param functionHostname string

var suffix = uniqueString(resourceGroup().id)

module blazor 'modules/web/blazor.bicep' = {
  name: 'blazor'
  params: {
    appInsightName: appInsightName
    location: location
    storageName: strName
    suffix: suffix
    functionHostname: functionHostname
  }
}

output webAppName string = blazor.outputs.webName
