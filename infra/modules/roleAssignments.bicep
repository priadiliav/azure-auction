@description('Name of an existing Container Registry to grant pull access on.')
param containerRegistryName string

@description('Principal (object) ID of the identity that should be able to pull images from the registry.')
param principalId string

@description('Name of an existing Storage Account to grant access on (used for a Function App\'s AzureWebJobsStorage).')
param storageAccountName string

@description('Principal (object) ID of the identity that should access the storage account.')
param storagePrincipalId string

@description('Name of an existing Service Bus namespace to grant send access on.')
param serviceBusNamespaceName string

@description('Principal (object) ID of the identity that should send messages to the Service Bus namespace.')
param serviceBusSenderPrincipalId string

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2026-01-01-preview' existing = {
  name: containerRegistryName
}

resource acrPullRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(containerRegistry.id, principalId, 'AcrPull')
  scope: containerRegistry
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
  dependsOn: [
    containerRegistry
  ]
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

var storageRoleDefinitionIds = [
  'b7e6dc6d-f1e8-4753-8033-0f276bb0955b' // Storage Blob Data Owner
  '974c5e8b-45b9-4653-ba55-5f855dd0fb88' // Storage Queue Data Contributor
  '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3' // Storage Table Data Contributor
]

resource storageRoleAssignments 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for roleDefinitionId in storageRoleDefinitionIds: {
  name: guid(storageAccount.id, storagePrincipalId, roleDefinitionId)
  scope: storageAccount
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleDefinitionId)
    principalId: storagePrincipalId
    principalType: 'ServicePrincipal'
  }
}]

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' existing = {
  name: serviceBusNamespaceName
}

resource serviceBusSenderRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(serviceBusNamespace.id, serviceBusSenderPrincipalId, 'AzureServiceBusDataSender')
  scope: serviceBusNamespace
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '69a216fc-b8fb-44d8-bc22-1f3c2cd27a39')
    principalId: serviceBusSenderPrincipalId
    principalType: 'ServicePrincipal'
  }
}
