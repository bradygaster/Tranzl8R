using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddApplicationInsightsMonitoring("Web Server");
builder.Host.UseOrleans(siloBuilder =>
{
    var storageConnectionString = builder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
    
    siloBuilder
        .AddApplicationInsightsTelemetryConsumer(builder.Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY"))
        .Configure<SiloOptions>(options => options.SiloName = "Web Server")
        .Configure<ClusterOptions>(clusterOptions =>
        {
            clusterOptions.ClusterId = "Cluster";
            clusterOptions.ServiceId = "Service";
        })
        .ConfigureEndpointsForAzureAppService(builder.Configuration)
        .UseAzureStorageClustering(storageOptions => storageOptions.ConnectionString = storageConnectionString)
        .AddAzureTableGrainStorageAsDefault(tableStorageOptions =>
        {
            tableStorageOptions.ConnectionString = storageConnectionString;
            tableStorageOptions.UseJson = true;
        });
});


var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();