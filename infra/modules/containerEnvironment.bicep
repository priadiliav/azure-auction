@description('Name of the Container Apps managed environment.')
param name string

@description('Azure region for the environment.')
param location string

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2026-01-01' = {
  name: name
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    zoneRedundant: false
    kedaConfiguration: {}
    daprConfiguration: {}
    customDomainConfiguration: {}
    workloadProfiles: [
      {
        workloadProfileType: 'Consumption'
        name: 'Consumption'
      }
    ]
    peerAuthentication: {
      mtls: {
        enabled: false
      }
    }
    peerTrafficConfiguration: {
      encryption: {
        enabled: false
      }
    }
    publicNetworkAccess: 'Enabled'
  }
}

output id string = containerAppsEnvironment.id
output principalId string = containerAppsEnvironment.identity.principalId
