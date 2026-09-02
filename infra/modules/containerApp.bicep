@description('Name of the Container App.')
param name string

@description('Azure region for the resource.')
param location string

@description('Resource ID of the Container Apps managed environment to run in.')
param environmentId string

@description('Login server of the Container Registry to pull images from (e.g. myregistry.azurecr.io).')
param registryServer string

@description('Full container image reference to run, e.g. myregistry.azurecr.io/app:tag. Defaults to a public placeholder so the first deploy succeeds before any image has been pushed to the registry.')
param containerImage string = 'mcr.microsoft.com/k8se/quickstart:latest'

@description('Port your app listens on inside the container. Used for both ingress routing and health probes.')
param targetPort int

@description('Environment variables for the container, as {name, value} objects. Build this array in the consuming main.bicep - this module has no knowledge of what your app actually needs.')
param environmentVariables array = []

@description('CPU cores allocated to the container, e.g. \'0.5\'.')
param cpu string = '0.5'

@description('Memory allocated to the container, e.g. \'1Gi\'.')
param memory string = '1Gi'

@description('Minimum number of replicas.')
param minReplicas int = 1

@description('Maximum number of replicas.')
param maxReplicas int = 1

@description('Container App kind. Use \'functionapp\' to run this as Azure Functions on Container Apps instead of a plain container app.')
param kind string = 'containerapps'

@description('Allowed CORS origins for the ingress. Empty array disables the CORS policy (e.g. when the app handles CORS itself, like ASP.NET Core\'s UseCors).')
param corsAllowedOrigins array = []

resource containerApp 'Microsoft.App/containerapps@2026-01-01' = {
  name: name
  location: location
  kind: kind
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: environmentId
    environmentId: environmentId
    workloadProfileName: 'Consumption'
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: targetPort
        exposedPort: 0
        transport: 'Auto'
        traffic: [
          {
            weight: 100
            latestRevision: true
          }
        ]
        allowInsecure: false
        ipSecurityRestrictions: []
        clientCertificateMode: 'Ignore'
        stickySessions: {
          affinity: 'none'
        }
        additionalPortMappings: []
        corsPolicy: empty(corsAllowedOrigins) ? null : {
          allowedOrigins: corsAllowedOrigins
          allowedMethods: [
            '*'
          ]
          allowedHeaders: [
            '*'
          ]
        }
      }
      registries: [
        {
          server: registryServer
          identity: 'system-environment'
        }
      ]
      runtime: {}
      maxInactiveRevisions: 100
      identitySettings: []
    }
    template: {
      containers: [
        {
          image: containerImage
          name: name
          command: []
          args: []
          env: environmentVariables
          resources: {
            cpu: json(cpu)
            memory: memory
          }
          probes: [
            {
              type: 'Liveness'
              failureThreshold: 3
              periodSeconds: 10
              successThreshold: 1
              tcpSocket: {
                port: targetPort
              }
              timeoutSeconds: 5
            }
            {
              type: 'Readiness'
              failureThreshold: 48
              periodSeconds: 5
              successThreshold: 1
              tcpSocket: {
                port: targetPort
              }
              timeoutSeconds: 5
            }
            {
              type: 'Startup'
              failureThreshold: 240
              initialDelaySeconds: 1
              periodSeconds: 1
              successThreshold: 1
              tcpSocket: {
                port: targetPort
              }
              timeoutSeconds: 3
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
        cooldownPeriod: 300
        pollingInterval: 30
        rules: kind == 'functionapp' ? [] : [
          {
            name: 'http-scaler'
            http: {
              metadata: {
                concurrentRequests: '10'
              }
            }
          }
        ]
      }
      volumes: []
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn
output principalId string = containerApp.identity.principalId
