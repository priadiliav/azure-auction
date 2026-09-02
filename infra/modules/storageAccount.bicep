@description('Name of the storage account. Must be globally unique, lowercase letters/numbers only, 3-24 chars.')
param name string

@description('Azure region for the resource.')
param location string

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: name
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
  }
}

output name string = storageAccount.name
output id string = storageAccount.id
