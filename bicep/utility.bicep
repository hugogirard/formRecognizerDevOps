param formRecognizerDevEndpoint string
param formRecognizerQAEndpoint string
param formRecognizerProdEndpoint string

param formRecognizerDevKey string
param formRecognizerQAKey string
param formRecognizerProdKey string

param devStorageCnxString string

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
    formRecognizerDevEndpoint: formRecognizerDevEndpoint
    formRecognizerDevKey: formRecognizerDevKey
    formRecognizerProdEndpoint: formRecognizerProdEndpoint
    formRecognizerProdKey: formRecognizerProdKey
    formRecognizerQAEndpoint: formRecognizerQAEndpoint
    formRecognizerQAKey: formRecognizerQAKey
    location: location
    strAccountApiVersion: storage.outputs.strAccountApiVersion
    strAccountId: storage.outputs.strAccountId
    strAccountName: storage.outputs.strAccountName
    suffix: suffix
    devStorageCnxString: devStorageCnxString
  }
}

output functionName string = function.outputs.functionName
