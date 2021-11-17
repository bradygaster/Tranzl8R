using Tranzl8R.TranslationService;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;
using Orleans.Runtime;
using static Tranzl8R.CognitiveServicesTranslator;
using Orleans.Runtime.Placement;

IHost host = Host
    .CreateDefaultBuilder(args)
#if DEBUG
    .ConfigureAppConfiguration(configurationBuilder =>
    {
        configurationBuilder.AddUserSecrets<Worker>();
    })
#endif
    .UseOrleans((hostBuilderContext, siloBuilder) =>
    {
        var storageConnectionString = hostBuilderContext.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

        IPAddress endpointAddress = IPAddress.Parse(hostBuilderContext.Configuration.GetValue<string>("WEBSITE_PRIVATE_IP"));

        var strPorts = hostBuilderContext.Configuration.GetValue<string>("WEBSITE_PRIVATE_PORTS").Split(',');

        if (strPorts.Length < 2) throw new Exception("Insufficient private ports configured.");

        int siloPort = int.Parse(strPorts[0]);
        int gatewayPort = int.Parse(strPorts[1]);
        int delta = new Random().Next(1, 100);

        siloBuilder
            .UseAzureStorageClustering(storageOptions => storageOptions.ConnectionString = storageConnectionString)
            .AddAzureTableGrainStorageAsDefault(tableStorageOptions =>
            {
                tableStorageOptions.ConnectionString = storageConnectionString;
                tableStorageOptions.UseJson = true;
            })
            .Configure<SiloOptions>(options => options.SiloName = "Worker Service")
            .Configure<ClusterOptions>(clusterOptions =>
            {
                clusterOptions.ClusterId = "Cluster";
                clusterOptions.ServiceId = "Service";
            })
            .ConfigureEndpoints(endpointAddress, siloPort + delta, gatewayPort + delta);
    })
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
