param location string
param suffix string
param environment: string

resource symbolicname 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: 'vault-${environment}-${suffix}'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
  }
}
