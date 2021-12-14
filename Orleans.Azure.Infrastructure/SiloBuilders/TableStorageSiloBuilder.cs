using Microsoft.Extensions.Configuration;

namespace Orleans.Hosting
{
    public class TableStorageSiloBuilder : AzureSiloBuilder
    {
        public override void Build(ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING")))
            {
                var azureStorageConnectionString = () => configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
                siloBuilder
                    .UseAzureStorageClustering(storageOptions => storageOptions.ConnectionString = azureStorageConnectionString())
                    .AddAzureTableGrainStorageAsDefault(tableStorageOptions =>
                    {
                        tableStorageOptions.ConnectionString = azureStorageConnectionString();
                        tableStorageOptions.UseJson = true;
                    });
            }

            if (!string.IsNullOrEmpty(configuration.GetValue<string>("ORLEANS_AZURE_STORAGE_CONNECTION_STRING")))
            {
                var azureStorageConnectionString = () => configuration.GetValue<string>("ORLEANS_AZURE_STORAGE_CONNECTION_STRING");
                siloBuilder
                    .UseAzureStorageClustering(storageOptions => storageOptions.ConnectionString = azureStorageConnectionString())
                    .AddAzureTableGrainStorageAsDefault(tableStorageOptions =>
                    {
                        tableStorageOptions.ConnectionString = azureStorageConnectionString();
                        tableStorageOptions.UseJson = true;
                    });
            }

            base.Build(siloBuilder, configuration);
        }
    }
}
