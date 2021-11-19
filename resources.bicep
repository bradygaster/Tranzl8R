
param resourceBaseName string = resourceGroup().name

resource vnet 'Microsoft.Network/virtualNetworks@2021-03-01' = {
  name: '${resourceBaseName}vnet'
  location: resourceGroup().location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '172.17.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'default'
        properties: {
          addressPrefix: '172.17.0.0/24'
          delegations: [
            {
              name: 'delegation'
              properties: {
                serviceName: 'Microsoft.Web/serverFarms'
              }
            }
          ]
        }
      }
    ]
  }
}

resource translator 'Microsoft.CognitiveServices/accounts@2021-04-30' = {
  name: '${resourceBaseName}translator'
  location: 'global'
  kind: 'TextTranslation'
  sku: {
    name: 'F0'
  }
}

resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02' = {
  name: '${resourceBaseName}ai'
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: '${resourceBaseName}strg'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_RAGRS'
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: '${resourceBaseName}plan'
  location: resourceGroup().location
  kind: 'app'
  sku: {
    name: 'S1'
    capacity: 1
  }
}

resource front_end 'Microsoft.Web/sites@2021-02-01' = {
  name: '${resourceBaseName}-ui'
  kind: 'app'
  location: resourceGroup().location
  properties: {
    serverFarmId: appServicePlan.id
    virtualNetworkSubnetId: vnet.properties.subnets[0].id
    siteConfig: {
      vnetPrivatePortsCount: 2
      webSocketsEnabled: true
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Development'
        }
        {
          name: 'AZURE_TRANSLATOR_ENDPOINT'
          value: translator.properties.endpoint
        }
        {
          name: 'AZURE_TRANSLATOR_SUBSCRIPTION_KEY'
          value: listKeys(translator.id, translator.apiVersion).key1
        }
        {
          name: 'AZURE_TRANSLATOR_LOCATION'
          value: translator.location
        }
        {
          name: 'AZURE_STORAGE_CONNECTION_STRING'
          value: format('DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${listKeys(storage.name, storage.apiVersion).keys[0].value};EndpointSuffix=core.windows.net')
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsightsComponents.properties.InstrumentationKey
        }
      ]
    }
  }
  dependsOn: [
    storage
    vnet
  ]
}

resource dashboard 'Microsoft.Web/sites@2021-02-01' = {
  name: '${resourceBaseName}-dashboard'
  kind: 'app'
  location: resourceGroup().location
  properties: {
    serverFarmId: appServicePlan.id
    virtualNetworkSubnetId: vnet.properties.subnets[0].id
    siteConfig: {
      vnetPrivatePortsCount: 2
      webSocketsEnabled: true
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Development'
        }
        {
          name: 'AZURE_TRANSLATOR_ENDPOINT'
          value: translator.properties.endpoint
        }
        {
          name: 'AZURE_TRANSLATOR_SUBSCRIPTION_KEY'
          value: listKeys(translator.id, translator.apiVersion).key1
        }
        {
          name: 'AZURE_TRANSLATOR_LOCATION'
          value: translator.location
        }
        {
          name: 'AZURE_STORAGE_CONNECTION_STRING'
          value: format('DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${listKeys(storage.name, storage.apiVersion).keys[0].value};EndpointSuffix=core.windows.net')
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsightsComponents.properties.InstrumentationKey
        }
      ]
    }
  }
  dependsOn: [
    storage
    vnet
  ]
}

