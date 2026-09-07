@description('Name of the Azure OpenAI resource.')
param name string

@description('Azure region for the resource. Must support the embedding model deployed here.')
param location string

@description('Name of the embedding model deployment (also used as the deployment name the app requests).')
param embeddingDeploymentName string = 'text-embedding-3-small'

resource account 'Microsoft.CognitiveServices/accounts@2025-06-01' = {
  name: name
  location: location
  kind: 'OpenAI'
  sku: {
    name: 'S0'
  }
  properties: {
    customSubDomainName: name
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: true
  }
}

resource embeddingDeployment 'Microsoft.CognitiveServices/accounts/deployments@2025-06-01' = {
  parent: account
  name: embeddingDeploymentName
  sku: {
    name: 'GlobalStandard'
    capacity: 1
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'text-embedding-3-small'
      version: '1'
    }
  }
}

output name string = account.name
output endpoint string = account.properties.endpoint
output embeddingDeploymentName string = embeddingDeployment.name
