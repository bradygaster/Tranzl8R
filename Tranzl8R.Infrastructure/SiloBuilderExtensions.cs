using Microsoft.Extensions.Configuration;
using Tranzl8R.Infrastructure;

namespace Orleans.Hosting
{
    public static class SiloBuilderExtensions
    {
        public static ISiloBuilder HostSiloInAzure(this ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            var webAppSiloBuilder = new AzureWebAppSiloBuilder();
            var containerAppsBuilder = new AzureContainerAppSiloBuilder();
            var clusterOptionsBuilder = new ClusterOptionsBuilder();
            var siloOptionsBuilder = new SiloOptionsBuilder();
            var tableStorageBuilder = new AzureStorageSiloBuilder();
            var appInsightsBuilder = new AzureApplicationInsightsSiloBuilder();

            webAppSiloBuilder.SetNextBuilder(containerAppsBuilder);
            containerAppsBuilder.SetNextBuilder(clusterOptionsBuilder);
            clusterOptionsBuilder.SetNextBuilder(siloOptionsBuilder);
            siloOptionsBuilder.SetNextBuilder(tableStorageBuilder);
            tableStorageBuilder.SetNextBuilder(appInsightsBuilder);

            webAppSiloBuilder.Build(siloBuilder, configuration);

            return siloBuilder;
        }
    }
}
