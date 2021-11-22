using Microsoft.Extensions.Configuration;
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

            return siloBuilder;
        }

        private static bool IsAppConfiguredToUseAzureStorage(this IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING")))
            {
                return false;
            }

            return true;
        }

        private static void ConfigureSiloToUseAzureStorage(this ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            var azureStorageConnectionString = configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
            siloBuilder
                .UseAzureStorageClustering(storageOptions => storageOptions.ConnectionString = azureStorageConnectionString)
                .AddAzureTableGrainStorageAsDefault(tableStorageOptions =>
                {
                    tableStorageOptions.ConnectionString = azureStorageConnectionString;
                    tableStorageOptions.UseJson = true;
                });
        }
    }
}
