using Microsoft.Extensions.Configuration;
using Orleans.Configuration;

namespace Orleans.Hosting
{
    public class ClusterOptionsClientBuilder : AzureSiloClientBuilder
    {
        public override void Build(IClientBuilder clientBuilder, IConfiguration configuration)
        {
            var clusterId = configuration.GetValue<string>("ORLEANS_CLUSTER_ID");
            var serviceId = configuration.GetValue<string>("ORLEANS_SERVICE_ID");

            clientBuilder.Configure<ClusterOptions>(clusterOptions =>
            {
                clusterOptions.ClusterId = string.IsNullOrEmpty(clusterId) ? "Cluster" : clusterId;
                clusterOptions.ServiceId = string.IsNullOrEmpty(serviceId) ? "Service" : serviceId;
            });

            base.Build(clientBuilder, configuration);
        }
    }
}
