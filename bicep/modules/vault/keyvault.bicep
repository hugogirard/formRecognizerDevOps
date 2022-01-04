param location string
param suffix string
param environment string
param spIdentity string


resource symbolicname 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
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
