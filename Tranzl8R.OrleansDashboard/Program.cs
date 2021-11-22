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
    siloBuilder
        .AddApplicationInsightsTelemetryConsumer(builder.Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY"))
        .Configure<SiloOptions>(options => options.SiloName = "Dashboard")
        .Configure<ClusterOptions>(clusterOptions =>
        {
            clusterOptions.ClusterId = "Cluster";
            clusterOptions.ServiceId = "Service";
        })
        .ConfigureEndpointsForAzureAppService(builder.Configuration)
        .UseAzureStorageClustering(storageOptions => storageOptions.ConnectionString = 
            builder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING"))
        .AddAzureTableGrainStorageAsDefault(tableStorageOptions =>
        {
            tableStorageOptions.ConnectionString = 
                builder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
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
