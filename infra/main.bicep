targetScope = 'resourceGroup'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Name of the container registry.')
param containerRegistryName string

@description('Name of the container apps managed environment.')
param containerEnvironmentName string

@description('Name of the container app.')
param containerAppName string

@description('Name of the container web app.')
param webAppName string = 'auction-web'

@description('Name of the container function app.')
param functionAppName string = 'auction-function'

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

module roleAssignments 'modules/roleAssignments.bicep' = {
  name: 'roleAssignments'
  params: {
    containerRegistryName: containerRegistry.outputs.name
    principalId: containerEnvironment.outputs.principalId
  }
}

module containerApp 'modules/containerApp.bicep' = {
  name: 'containerApp'
  params: {
    name: containerAppName
    location: location
    registryServer: containerRegistry.outputs.loginServer
    environmentId: containerEnvironment.outputs.id
    targetPort: 8080
  }
}

// module functionApp 'modules/functionApp.bicep' = {
//   name: 'functionApp'
//   params: {
//     name: functionAppName
//     location: location
//     containerRegistryName: containerRegistry.outputs.name
//     containerRegistryLoginServer: containerRegistry.outputs.loginServer
//     containerEnvironmentId: containerEnvironment.outputs.id
//   }
// }
