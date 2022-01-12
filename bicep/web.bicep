param location string
param strName string
param appInsightName string

var suffix = uniqueString(resourceGroup().id)

module blazor 'modules/web/blazor.bicep' = {
  name: 'blazor'
  params: {
    appInsightName: appInsightName
    location: location
    storageName: strName
    suffix: suffix
  }
}

output webAppName string = blazor.outputs.webName
