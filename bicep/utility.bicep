
param devStorageName string
param devFormRecognizerName string
param qaFormRecognizerName string
param prodFormRecognizerName string

param devResourceGroupName string
param qaResourceGroupName string
param prodResourceGroupName string

var location = resourceGroup().location
var suffix = uniqueString(resourceGroup().id)

module insight 'modules/insights/insights.bicep' = {
  name: 'insight'
  params: {
    location: location
    suffix: suffix
  }
}  

module storage 'modules/storage/storage-function.bicep' = {
  name: 'storage'
  params: {     
    location: location
    suffix: suffix
  }
}

module function 'modules/function/function.bicep' = {
  name: 'function'
  params: {
    appInsightCnxString: insight.outputs.appInsightCnxString
    appInsightKey: insight.outputs.appInsightKey
    location: location
    devStorageName: devStorageName
    devFormRecognizerName: devFormRecognizerName
    qaFormRecognizerName: qaFormRecognizerName
    prodFormRecognizerName: prodFormRecognizerName
    devResourceGroupName: devResourceGroupName
    qaResourceGroupName: qaResourceGroupName
    prodResourceGroupName: prodResourceGroupName
    strAccountApiVersion: storage.outputs.strAccountApiVersion
    strAccountId: storage.outputs.strAccountId
    strAccountName: storage.outputs.strAccountName    
    suffix: suffix    
  }
}

output functionName string = function.outputs.functionName
output storageName string = storage.outputs.strAccountName
output appInsightName string = insight.outputs.appInsightName
