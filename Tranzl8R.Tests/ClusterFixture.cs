using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using Orleans.TestingHost;
using System;

namespace Tranzl8R.Tests
{
    public class ClusterFixture : IDisposable
    {
        public ClusterFixture()
        {
            var builder = new TestClusterBuilder();
            builder.AddSiloBuilderConfigurator<TestSiloConfigurations>();
            this.Cluster = builder.Build();
            this.Cluster.Deploy();
        }

        public void Dispose()
        {
            this.Cluster.StopAllSilos();
        }

        public TestCluster Cluster { get; private set; }
    }

    public class TestSiloConfigurations : ISiloConfigurator
    {
        private IConfigurationRoot _configuration;
        public TestSiloConfigurations()
        {
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<ClusterFixture>()
                .Build();
        }

        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder.AddAzureTableGrainStorageAsDefault((storageOptions) =>
            {
                storageOptions.UseJson = true;
                storageOptions.ConnectionString = _configuration["AZURE_STORAGE_CONNECTION_STRING"];
            });

            siloBuilder.ConfigureServices(services => {
                services.AddHttpClient();
                services.AddSingleton<ITranslationServer, CognitiveServicesTranslationServer>();
                services.AddSingleton<IConfiguration>(_configuration);
            });
        }
    }
}
