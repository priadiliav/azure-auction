targetScope = 'resourceGroup'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Name of the container registry.')
param containerRegistryName string

@description('Name of the container apps managed environment.')
param containerEnvironmentName string

@description('Name of the container app.')
param containerAppName string

@description('Name of the container function app.')
param functionAppName string = 'auction-function'

@description('Name of the storage account backing the function app (AzureWebJobsStorage).')
param functionStorageAccountName string

param imageNameAPI string = 'auction-webapi'
param imageNameFunction string = 'auction-functions'

module containerRegistry 'modules/containerRegistry.bicep' = {
  name: 'containerRegistry'
  params: {
    name: containerRegistryName
    location: location
  }
}

module containerEnvironment 'modules/containerEnvironment.bicep' = {
  name: 'containerEnvironment'
  params: {
    name: containerEnvironmentName
    location: location
  }
}

module containerApp 'modules/containerApp.bicep' = {
  name: 'containerApp'
  params: {
    name: containerAppName
    location: location
    registryServer: containerRegistry.outputs.loginServer
    environmentId: containerEnvironment.outputs.id
    containerImage: '${containerRegistry.outputs.loginServer}/${imageNameAPI}:latest'
    targetPort: 8080
  }
}

module functionStorageAccount 'modules/storageAccount.bicep' = {
  name: 'functionStorageAccount'
  params: {
    name: functionStorageAccountName
    location: location
  }
}

module functionApp 'modules/containerApp.bicep' = {
  name: 'functionApp'
  params: {
    name: functionAppName
    location: location
    kind: 'functionapp'
    registryServer: containerRegistry.outputs.loginServer
    environmentId: containerEnvironment.outputs.id
    containerImage: '${containerRegistry.outputs.loginServer}/${imageNameFunction}:latest'
    targetPort: 80
    environmentVariables: [
      {
        name: 'AzureWebJobsStorage__accountName'
        value: functionStorageAccount.outputs.name
      }
      {
        name: 'AzureWebJobsStorage__credential'
        value: 'managedidentity'
      }
    ]
  }
}

module roleAssignments 'modules/roleAssignments.bicep' = {
  name: 'roleAssignments'
  params: {
    containerRegistryName: containerRegistry.outputs.name
    principalId: containerEnvironment.outputs.principalId
    storageAccountName: functionStorageAccount.outputs.name
    storagePrincipalId: functionApp.outputs.principalId
  }
}
