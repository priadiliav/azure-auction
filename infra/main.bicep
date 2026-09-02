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

@description('Name of the static web app.')
param staticWebAppName string

@description('URL of the repository to deploy from.')
param repositoryUrl string

@description('Branch of the repository to deploy from. Defaults to \'main\'.')
param branch string

@description('GitHub personal access token (repo + workflow scopes) used to set up the CI/CD integration.')
@secure()
param repositoryToken string

param imageNameAPI string = 'auction-webapi'
param imageNameFunction string = 'auction-functions'

@description('Name of the Log Analytics workspace backing platform logs and Application Insights.')
param logAnalyticsWorkspaceName string = 'auction-logs'

@description('Name of the Application Insights component.')
param appInsightsName string = 'auction-insights'

@description('Name of the Service Bus namespace.')
param serviceBusNamespaceName string = 'auction-bus'

@description('Name of the logical SQL Server.')
param sqlServerName string = 'auction-sql'

@description('Name of the SQL Database.')
param sqlDatabaseName string = 'auction-db'

@description('AAD object id of the Entra ID admin for the SQL Server.')
param sqlAadAdminObjectId string

@description('Display name of the Entra ID admin (shown in the portal only).')
param sqlAadAdminLogin string

module containerRegistry 'modules/containerRegistry.bicep' = {
  name: 'containerRegistry'
  params: {
    name: containerRegistryName
    location: location
  }
}

module appMonitor 'modules/appMonitor.bicep' = {
  name: 'appMonitor'
  params: {
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    appInsightsName: appInsightsName
    location: location
  }
}

module containerEnvironment 'modules/containerEnvironment.bicep' = {
  name: 'containerEnvironment'
  params: {
    name: containerEnvironmentName
    location: location
    logAnalyticsWorkspaceName: appMonitor.outputs.logAnalyticsWorkspaceName
  }
}

module serviceBus 'modules/serviceBus.bicep' = {
  name: 'serviceBus'
  params: {
    name: serviceBusNamespaceName
    location: location
  }
}

module sqlDatabase 'modules/sqlDatabase.bicep' = {
  name: 'sqlDatabase'
  params: {
    serverName: sqlServerName
    databaseName: sqlDatabaseName
    location: location
    aadAdminObjectId: sqlAadAdminObjectId
    aadAdminLogin: sqlAadAdminLogin
  }
}

module staticSites 'modules/staticWebApp.bicep' = {
  name: 'staticSites'
  params: {
    name: staticWebAppName
    location: location
    repositoryUrl: repositoryUrl
    branch: branch
    repositoryToken: repositoryToken
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
    environmentVariables: [
      {
        name: 'Cors__AllowedOrigins__0'
        value: 'https://${staticSites.outputs.defaultHostname}'
      }
      {
        name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
        value: appMonitor.outputs.appInsightsConnectionString
      }
      {
        name: 'ServiceBus__FullyQualifiedNamespace'
        value: serviceBus.outputs.fullyQualifiedNamespace
      }
      {
        name: 'ConnectionStrings__AuctionDb'
        value: 'Server=tcp:${sqlDatabase.outputs.serverFqdn},1433;Database=${sqlDatabase.outputs.databaseName};Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;'
      }
    ]
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
      {
        name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
        value: appMonitor.outputs.appInsightsConnectionString
      }
      {
        name: 'ServiceBus__FullyQualifiedNamespace'
        value: serviceBus.outputs.fullyQualifiedNamespace
      }
      {
        name: 'ServiceBusConnection__fullyQualifiedNamespace'
        value: serviceBus.outputs.fullyQualifiedNamespace
      }
      {
        name: 'ConnectionStrings__AuctionDb'
        value: 'Server=tcp:${sqlDatabase.outputs.serverFqdn},1433;Database=${sqlDatabase.outputs.databaseName};Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;'
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
    serviceBusNamespaceName: serviceBus.outputs.name
    serviceBusSenderPrincipalId: containerApp.outputs.principalId
    serviceBusProcessorPrincipalId: functionApp.outputs.principalId
  }
}
