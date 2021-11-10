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
builder.Host.UseOrleans(siloBuilder =>
{
    var storageConnectionString = builder.Configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");

    IPAddress endpointAddress = IPAddress.Parse(builder.Configuration.GetValue<string>("WEBSITE_PRIVATE_IP"));

    var strPorts = builder.Configuration.GetValue<string>("WEBSITE_PRIVATE_PORTS").Split(',');

    if (strPorts.Length < 2) throw new Exception("Insufficient private ports configured."); 
    
    int siloPort = int.Parse(strPorts[0]);
    int gatewayPort = int.Parse(strPorts[1]);

    siloBuilder
        .Configure<SiloOptions>(options => options.SiloName = "Web Server")
        .Configure<ClusterOptions>(clusterOptions =>
        {
            clusterOptions.ClusterId = "Cluster";
            clusterOptions.ServiceId = "Service";
        })
        .Configure<EndpointOptions>(endpointOptions =>
        {
            endpointOptions.AdvertisedIPAddress = IPAddress.Loopback;
            endpointOptions.SiloPort = siloPort;
            endpointOptions.GatewayPort = gatewayPort;
        })
        .UseAzureStorageClustering(storageOptions => storageOptions.ConnectionString = storageConnectionString)
        .AddAzureTableGrainStorageAsDefault(tableStorageOptions =>
        {
            tableStorageOptions.ConnectionString = storageConnectionString;
            tableStorageOptions.UseJson = true;
        })
        .ConfigureApplicationParts(applicationParts =>
                applicationParts.AddApplicationPart(typeof(CognitiveServicesTranslator).Assembly).WithReferences())
        .UseDashboard();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();