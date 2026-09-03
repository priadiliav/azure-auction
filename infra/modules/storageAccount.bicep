@description('Name of the storage account. Must be globally unique, lowercase letters/numbers only, 3-24 chars.')
param name string

@description('Azure region for the resource.')
param location string

@description('Names of blob containers to create in this storage account.')
param containerNames array = []

@description('Allowed CORS origins for direct browser access to the blob service. Empty array disables CORS.')
param corsAllowedOrigins array = []

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

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: empty(corsAllowedOrigins) ? {} : {
    cors: {
      corsRules: [
        {
          allowedOrigins: corsAllowedOrigins
          allowedMethods: [
            'PUT'
            'GET'
            'HEAD'
            'OPTIONS'
          ]
          allowedHeaders: [
            '*'
          ]
          exposedHeaders: [
            '*'
          ]
          maxAgeInSeconds: 3600
        }
      ]
    }
  }
}

resource containers 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = [for containerName in containerNames: {
  parent: blobServices
  name: containerName
  properties: {
    publicAccess: 'None'
  }
}]

output name string = storageAccount.name
output id string = storageAccount.id
