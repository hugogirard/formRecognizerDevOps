param location string
param suffix string
param environment string
param spIdentity string

param frmRecognizerEndpoint string
param frmKey string

resource keyvault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: 'vault-${environment}-${suffix}'
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
