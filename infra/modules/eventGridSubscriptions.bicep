@description('Name of an existing Storage Account to create the event subscription on.')
param storageAccountName string

@description('Name of the blob container to scope the subscription to.')
param containerName string

@description('Base HTTPS URL of the Function App (no path), e.g. https://auction-function.example.azurecontainerapps.io')
param functionAppBaseUrl string

@description('Name of the blob-triggered function to deliver events to (the [Function(...)] name).')
param functionName string

@description('The Function App\'s "blobs_extension" system key, required to authenticate Event Grid\'s webhook call to the blob trigger. Fetch it yourself (e.g. from the azure-webjobs-secrets storage container) - this is never read or logged by tooling.')
@secure()
param blobsExtensionSystemKey string

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource blobCreatedSubscription 'Microsoft.EventGrid/eventSubscriptions@2022-06-15' = {
  name: '${storageAccountName}-${containerName}-blob-created'
  scope: storageAccount
  properties: {
    destination: {
      endpointType: 'WebHook'
      properties: {
        endpointUrl: '${functionAppBaseUrl}/runtime/webhooks/blobs?functionName=${functionName}&code=${blobsExtensionSystemKey}'
      }
    }
    eventDeliverySchema: 'EventGridSchema'
    filter: {
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
      subjectBeginsWith: '/blobServices/default/containers/${containerName}/'
    }
  }
}
