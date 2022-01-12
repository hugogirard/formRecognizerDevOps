param locations string
param strName string
param appInsightName string

var suffix = uniqueString(resourceGroup().id)

module blazor 'modules/web/blazor.bicep' = {
  name: 'blazor'
  params: {
    appInsightName: appInsightName
    location: locations
    storageName: strName
    suffix: suffix
  }
}

