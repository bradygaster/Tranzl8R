using Orleans;
using Orleans.Hosting;
using Tranzl8R.TranslationService;

IHost host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
    })
    .UseOrleans((hostBuilderContext, siloBuilder) =>
    {
        siloBuilder.HostSiloInAzure(hostBuilderContext.Configuration);
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
