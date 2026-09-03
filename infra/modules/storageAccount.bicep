@description('Name of the storage account. Must be globally unique, lowercase letters/numbers only, 3-24 chars.')
param name string

@description('Azure region for the resource.')
param location string

@description('Blob containers to create, each as { name, publicAccess: \'None\' | \'Blob\' | \'Container\' }. publicAccess defaults to None.')
param containers array = []

@description('Allowed CORS origins for direct browser access to the blob service. Empty array disables CORS.')
param corsAllowedOrigins array = []

@description('Allow anonymous public read access to blobs in containers configured for it. Must be true for any container-level publicAccess setting to take effect.')
param allowBlobPublicAccess bool = false

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: name
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: allowBlobPublicAccess
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

resource containerResources 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = [for container in containers: {
  parent: blobServices
  name: container.name
  properties: {
    publicAccess: container.?publicAccess ?? 'None'
  }
}]

output name string = storageAccount.name
output id string = storageAccount.id
