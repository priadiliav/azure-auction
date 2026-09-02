@description('Name of the Azure Static Web App.')
param name string

@description('Azure region for the resource.')
param location string

@description('GitHub repository URL to link for CI/CD.')
param repositoryUrl string

@description('Branch to deploy from.')
param branch string

@secure()
@description('GitHub personal access token (repo + workflow scopes) used to set up the CI/CD integration.')
param repositoryToken string

resource staticSites 'Microsoft.Web/staticSites@2024-11-01' = {
  name: name
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {
    repositoryUrl: repositoryUrl
    branch: branch
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
    provider: 'GitHub'
    repositoryToken: repositoryToken
    enterpriseGradeCdnStatus: 'Disabled'
  }
}

output name string = staticSites.name
output defaultHostname string = staticSites.properties.defaultHostname
