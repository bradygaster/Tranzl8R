using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;
using Tranzl8R;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddApplicationInsightsMonitoring("Orleans Dashboard");

builder.Host.UseOrleans(siloBuilder =>
{
    var storageConnectionString = builder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

    IPAddress endpointAddress = IPAddress.Parse(builder.Configuration.GetValue<string>("WEBSITE_PRIVATE_IP"));

    var strPorts = builder.Configuration.GetValue<string>("WEBSITE_PRIVATE_PORTS").Split(',');

    if (strPorts.Length < 2) throw new Exception("Insufficient private ports configured.");

    int siloPort = int.Parse(strPorts[0]);
    int gatewayPort = int.Parse(strPorts[1]);

    siloBuilder
        .AddApplicationInsightsTelemetryConsumer(builder.Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY"))
        .Configure<SiloOptions>(options => options.SiloName = "Dashboard")
        .Configure<ClusterOptions>(clusterOptions =>
        {
            clusterOptions.ClusterId = "Cluster";
            clusterOptions.ServiceId = "Service";
        })
        .ConfigureEndpoints(endpointAddress, siloPort, gatewayPort)
        .UseAzureStorageClustering(storageOptions => storageOptions.ConnectionString = storageConnectionString)
        .AddAzureTableGrainStorageAsDefault(tableStorageOptions =>
        {
            tableStorageOptions.ConnectionString = storageConnectionString;
            tableStorageOptions.UseJson = true;
        })
        .ConfigureApplicationParts(applicationParts => applicationParts.AddApplicationPart(typeof(CognitiveServicesTranslator).Assembly).WithReferences())
        .UseDashboard(dashboardOptions => dashboardOptions.HostSelf = false);
});

builder.Services.AddServicesForSelfHostedDashboard();
var app = builder.Build();
app.UseOrleansDashboard();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
