using Microsoft.Extensions.Configuration;

namespace Orleans.Hosting
{
    internal class AzureStorageSiloClientBuillder : AzureSiloClientBuilder
    {
        public override void Build(IClientBuilder clientBuilder, IConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(configuration.GetValue<string>("ORLEANS_AZURE_STORAGE_CONNECTION_STRING")))
            {
                var azureStorageConnectionString = () => configuration.GetValue<string>("ORLEANS_AZURE_STORAGE_CONNECTION_STRING");
                clientBuilder.UseAzureStorageClustering(options =>
                {
                    options.ConnectionString = azureStorageConnectionString();
                });
            }

            base.Build(clientBuilder, configuration);
        }
    }
}
