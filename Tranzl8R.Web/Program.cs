using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddApplicationInsightsMonitoring("Web Server");

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.HostSiloInAzure(builder.Configuration);
});


var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();