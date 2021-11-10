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