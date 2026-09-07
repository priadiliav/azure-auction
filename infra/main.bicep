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

@description('Name of the Azure SignalR Service resource.')
param signalRName string = 'auction-signalr'

@description('Name of the storage account backing item image uploads.')
param itemBlobStorageAccountName string = 'auctionitemsblob'

@description('The Function App\'s "blobs_extension" system key, needed to authenticate Event Grid\'s webhook call to the item-image blob trigger. Fetch it yourself (e.g. from the azure-webjobs-secrets storage container) and pass it at deploy time - never committed or logged.')
@secure()
param blobsExtensionSystemKey string

@description('Google OAuth 2.0 Client ID used to validate Google Sign-In ID tokens. Not a secret - it is also embedded in the frontend.')
param googleClientId string

@description('Name of the Azure OpenAI resource backing item embeddings/recommendations.')
param openAiName string = 'auction-openai'

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

module signalR 'modules/signalR.bicep' = {
  name: 'signalR'
  params: {
    name: signalRName
    location: location
  }
}

module openAi 'modules/openAi.bicep' = {
  name: 'openAi'
  params: {
    name: openAiName
    location: location
  }
}

module itemBlobStorageAccount 'modules/storageAccount.bicep' = {
  name: 'itemBlobStorageAccount'
  params: {
    name: itemBlobStorageAccountName
    location: location
    allowBlobPublicAccess: true
    containers: [
      {
        name: 'items'
        publicAccess: 'Blob'
      }
    ]
    corsAllowedOrigins: [
      'https://${staticSites.outputs.defaultHostname}'
    ]
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
        name: 'BlobStorage__AccountName'
        value: itemBlobStorageAccount.outputs.name
      }
      {
        name: 'ConnectionStrings__AuctionDb'
        value: 'Server=tcp:${sqlDatabase.outputs.serverFqdn},1433;Database=${sqlDatabase.outputs.databaseName};Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;'
      }
      {
        name: 'Authentication__Google__ClientId'
        value: googleClientId
      }
      {
        name: 'AzureOpenAI__Endpoint'
        value: openAi.outputs.endpoint
      }
      {
        name: 'AzureOpenAI__EmbeddingDeploymentName'
        value: openAi.outputs.embeddingDeploymentName
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
    corsAllowedOrigins: [
      'https://${staticSites.outputs.defaultHostname}'
    ]
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
        name: 'BlobStorage__AccountName'
        value: itemBlobStorageAccount.outputs.name
      }
      {
        name: 'ConnectionStrings__AuctionDb'
        value: 'Server=tcp:${sqlDatabase.outputs.serverFqdn},1433;Database=${sqlDatabase.outputs.databaseName};Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;'
      }
      {
        name: 'AzureSignalRConnectionString__serviceUri'
        value: 'https://${signalR.outputs.hostName}'
      }
      {
        name: 'AzureSignalRConnectionString__credential'
        value: 'managedIdentity'
      }
      {
        name: 'Authentication__Google__ClientId'
        value: googleClientId
      }
      {
        name: 'AzureOpenAI__Endpoint'
        value: openAi.outputs.endpoint
      }
      {
        name: 'AzureOpenAI__EmbeddingDeploymentName'
        value: openAi.outputs.embeddingDeploymentName
      }
    ]
  }
}

module itemBlobCreatedSubscription 'modules/eventGridSubscriptions.bicep' = {
  name: 'itemBlobCreatedSubscription'
  params: {
    storageAccountName: itemBlobStorageAccount.outputs.name
    containerName: 'items'
    functionAppBaseUrl: 'https://${functionApp.outputs.fqdn}'
    functionName: 'ItemImageEventGridTrigger'
    blobsExtensionSystemKey: blobsExtensionSystemKey
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
    signalRName: signalR.outputs.name
    signalRPrincipalId: functionApp.outputs.principalId
    itemBlobStorageAccountName: itemBlobStorageAccount.outputs.name
    itemBlobPrincipalId: functionApp.outputs.principalId
    openAiName: openAi.outputs.name
    openAiPrincipalId: functionApp.outputs.principalId
  }
}
