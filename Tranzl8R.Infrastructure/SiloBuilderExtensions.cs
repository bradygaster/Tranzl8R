using Microsoft.Extensions.Configuration;
using Orleans.Configuration;
using System.Net;

namespace Orleans.Hosting
{
    public static class SiloBuilderExtensions
    {
        public static ISiloBuilder HostSiloInAzure(this ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            if (configuration.GetValue<string>("WEBSITE_PRIVATE_IP") != null &&
                configuration.GetValue<string>("WEBSITE_PRIVATE_PORTS") != null)
            {
                // presume the app is running in Web Apps on App Service and start up
                IPAddress endpointAddress = IPAddress.Parse(configuration.GetValue<string>("WEBSITE_PRIVATE_IP"));

                var strPorts = configuration.GetValue<string>("WEBSITE_PRIVATE_PORTS").Split(',');

                if (strPorts.Length < 2) throw new Exception("Insufficient private ports configured.");

                int siloPort = int.Parse(strPorts[0]);
                int gatewayPort = int.Parse(strPorts[1]);

                siloBuilder.ConfigureEndpoints(endpointAddress, siloPort, gatewayPort);
            }

            // wire up for azure storage if appropriate
            if(configuration.IsAppConfiguredToUseAzureStorage())
            {
                siloBuilder.ConfigureSiloToUseAzureStorage(configuration);
            }

            // wire up for azure monitoring if appropriate
            if (configuration.IsAppConfiguredToUseAzureMonitoring())
            {
                siloBuilder.ConfigureSiloToUseAzureMonitoring(configuration);
            }

            // wire up the detailed configuration
            siloBuilder.ConfigureSiloOptions(configuration);
            siloBuilder.ConfigureClusterOptions(configuration);

            return siloBuilder;
        }

        private static bool IsAppConfiguredToUseAzureStorage(this IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.GetValue<string>("ORLEANS_AZURE_STORAGE_CONNECTION_STRING")))
            {
                return false;
            }

            return true;
        }

        private static void ConfigureSiloToUseAzureStorage(this ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            var azureStorageConnectionString = configuration.GetValue<string>("ORLEANS_AZURE_STORAGE_CONNECTION_STRING");
            siloBuilder
                .UseAzureStorageClustering(storageOptions => storageOptions.ConnectionString = azureStorageConnectionString)
                .AddAzureTableGrainStorageAsDefault(tableStorageOptions =>
                {
                    tableStorageOptions.ConnectionString = azureStorageConnectionString;
                    tableStorageOptions.UseJson = true;
                });
        }

        private static bool IsAppConfiguredToUseAzureMonitoring(this IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY")))
            {
                return false;
            }

            return true;
        }

        private static void ConfigureSiloToUseAzureMonitoring(this ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            var azureMonitoringInstrumentationKey = configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");
            siloBuilder
                .AddApplicationInsightsTelemetryConsumer(configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY"));
        }

        private static void ConfigureSiloOptions(this ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            var siloName = configuration.GetValue<string>("ORLEANS_SILO_NAME");
            if(!string.IsNullOrEmpty(siloName))
            {
                siloBuilder.Configure<SiloOptions>(options => options.SiloName = siloName);
            }
            else
            {
                throw new Exception("Silo Name not configured.");
            }
        }

        private static void ConfigureClusterOptions(this ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            var clusterId = configuration.GetValue<string>("ORLEANS_CLUSTER_ID");
            var serviceId = configuration.GetValue<string>("ORLEANS_SERVICE_ID");
            if (!string.IsNullOrEmpty(clusterId) && !string.IsNullOrEmpty(serviceId))
            {
                siloBuilder.Configure<ClusterOptions>(clusterOptions =>
                {
                    clusterOptions.ClusterId = clusterId;
                    clusterOptions.ServiceId = serviceId;
                });
            }
            else
            {
                throw new Exception("Cluster and Service IDs not configured.");
            }
        }
    }
}
