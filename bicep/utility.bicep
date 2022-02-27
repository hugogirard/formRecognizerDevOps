
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
param devStorageName string
param devFormRecognizerName string
param qaFormRecognizerName string
param prodFormRecognizerName string

param devResourceGroupName string
param qaResourceGroupName string
param prodResourceGroupName string
param location string = resourceGroup().location

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
output functionHostname string = function.outputs.functionHostname
output storageName string = storage.outputs.strAccountName
output appInsightName string = insight.outputs.appInsightName
