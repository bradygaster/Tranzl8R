using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;
using Tranzl8R;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddApplicationInsightsMonitoring("Web Server");
builder.Host.UseOrleans(siloBuilder =>
{
    var storageConnectionString = builder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

    IPAddress endpointAddress = IPAddress.Parse(builder.Configuration.GetValue<string>("WEBSITE_PRIVATE_IP"));

    var strPorts = builder.Configuration.GetValue<string>("WEBSITE_PRIVATE_PORTS").Split(',');

    if (strPorts.Length < 2) throw new Exception("Insufficient private ports configured."); 
    
    int siloPort = int.Parse(strPorts[0]);
    int gatewayPort = int.Parse(strPorts[1]);
    int dashboardPort = (strPorts.Length > 2) ? int.Parse(strPorts[2]) : 8080;

    siloBuilder
        .AddApplicationInsightsTelemetryConsumer(builder.Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY"))
        .Configure<SiloOptions>(options => options.SiloName = "Web Server")
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
        ;
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();