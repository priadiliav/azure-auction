@description('Name of the Azure SignalR Service resource.')
param name string

@description('Azure region for the resource.')
param location string

resource signalR 'Microsoft.SignalRService/signalR@2023-02-01' = {
  name: name
  location: location
  kind: 'SignalR'
  sku: {
    name: 'Free_F1'
    capacity: 1
  }
  properties: {
    features: [
      {
        flag: 'ServiceMode'
        value: 'Serverless'
      }
    ]
    cors: {
      allowedOrigins: [
        '*'
      ]
    }
  }
}

output name string = signalR.name
output hostName string = signalR.properties.hostName
