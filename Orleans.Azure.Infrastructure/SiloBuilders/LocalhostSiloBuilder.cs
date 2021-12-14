using Microsoft.Extensions.Configuration;

namespace Orleans.Hosting
{
    internal class LocalhostSiloBuilder : AzureSiloBuilder
    {
        public override void Build(ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING")) &&
                string.IsNullOrEmpty(configuration.GetValue<string>("ORLEANS_AZURE_STORAGE_CONNECTION_STRING")) &&
                string.IsNullOrEmpty(configuration.GetValue<string>("ORLEANS_SILO_PORT")) &&
                string.IsNullOrEmpty(configuration.GetValue<string>("ORLEANS_GATEWAY_PORT")) &&
                string.IsNullOrEmpty(configuration.GetValue<string>("WEBSITE_PRIVATE_IP")) &&
                string.IsNullOrEmpty(configuration.GetValue<string>("WEBSITE_PRIVATE_PORTS")))
                // check for other clustering configurations, and if none are found...)
            {
                siloBuilder.UseLocalhostClustering();
            }

            base.Build(siloBuilder, configuration);
        }
    }
}