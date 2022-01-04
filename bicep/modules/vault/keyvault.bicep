param location string
param suffix string
param environmentName string
param spIdentity string

param frmRecognizerEndpoint string
param frmKey string

param strDocumentName string
param strDocumentId string
param strDocumentApiVersion string

var strCnxString = 'DefaultEndpointsProtocol=https;AccountName=${strDocumentName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(strDocumentId, strDocumentApiVersion).keys[0].value}'

resource keyvault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: 'vault-${environmentName}-${suffix}'
  location: location
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: spIdentity
        permissions: {
          secrets: [
            'all'
          ]
        }
      }
    ]
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableSoftDelete: false
  }
}

resource secretEndpoint 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: 'frmEndpoint'
  parent: keyvault
  properties: {
    value: frmRecognizerEndpoint
  }
}

resource secretKey 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: 'frmKey'
  parent: keyvault
  properties: {
    value: frmKey
  }
}

resource secretStorageCnxString 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = if (environmentName == 'dev') { 
  name: 'frmCnxString'
  parent: keyvault
  properties: {
    value: strCnxString
  }
}

output keyvaultName string = keyvault.name
