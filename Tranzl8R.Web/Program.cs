using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Orleans;
using Orleans.Hosting;
using Tranzl8R;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder
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