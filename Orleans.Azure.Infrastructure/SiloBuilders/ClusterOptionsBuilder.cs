using Microsoft.Extensions.Configuration;
using Orleans.Configuration;

namespace Orleans.Hosting
{
    public class ClusterOptionsBuilder : AzureSiloBuilder
    {
        public override void Build(ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            var clusterId = configuration.GetValue<string>("ORLEANS_CLUSTER_ID");
            var serviceId = configuration.GetValue<string>("ORLEANS_SERVICE_ID");

            siloBuilder.Configure<ClusterOptions>(clusterOptions =>
            {
                clusterOptions.ClusterId = string.IsNullOrEmpty(clusterId) ? "Cluster" : clusterId;
                clusterOptions.ServiceId = string.IsNullOrEmpty(serviceId) ? "Service" : serviceId;
            });

            base.Build(siloBuilder, configuration);
        }
    }
}
