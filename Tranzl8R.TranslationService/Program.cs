using Tranzl8R.TranslationService;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

IHost host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .ConfigureAppConfiguration(configurationBuilder =>
    {
        configurationBuilder.AddUserSecrets<Worker>();
    })
    .UseOrleans((hostBuilderContext, siloBuilder) =>
    {
        var storageConnectionString = hostBuilderContext.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

        IPAddress endpointAddress = IPAddress.Parse(hostBuilderContext.Configuration.GetValue<string>("WEBSITE_PRIVATE_IP"));

        var strPorts = hostBuilderContext.Configuration.GetValue<string>("WEBSITE_PRIVATE_PORTS").Split(',');

        int siloPort = int.Parse(strPorts[0]);
        int gatewayPort = int.Parse(strPorts[1]);
        int delta = 11;

        siloBuilder
            .Configure<SiloOptions>(options => options.SiloName = "Worker Service")
            .Configure<ClusterOptions>(clusterOptions =>
            {
                clusterOptions.ClusterId = "Cluster";
                clusterOptions.ServiceId = "Service";
            })
            .Configure<EndpointOptions>(endpointOptions =>
            {
                endpointOptions.AdvertisedIPAddress = IPAddress.Loopback;
                endpointOptions.SiloPort = siloPort + delta;
                endpointOptions.GatewayPort = gatewayPort + delta;
            })
            .UseAzureStorageClustering(storageOptions =>
            {
                storageOptions.ConnectionString = storageConnectionString;
            })
            .AddAzureTableGrainStorageAsDefault(tableStorageOptions =>
            {
                tableStorageOptions.ConnectionString = storageConnectionString;
                tableStorageOptions.UseJson = true;
            });
    })
    .Build();

await host.RunAsync();
