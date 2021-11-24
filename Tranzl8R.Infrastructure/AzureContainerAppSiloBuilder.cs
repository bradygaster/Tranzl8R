using Microsoft.Extensions.Configuration;
using Orleans.Hosting;
using System.Net;

namespace Tranzl8R.Infrastructure
{
    public class AzureContainerAppSiloBuilder : AzureSiloBuilder
    {
        public override void Build(ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            if (configuration.GetValue<string>("ORLEANS_SILO_PORT") != null &&
                configuration.GetValue<string>("ORLEANS_GATEWAY_PORT") != null)
            {
                siloBuilder.ConfigureEndpoints(Dns.GetHostName(),
                    configuration.GetValue<int>("ORLEANS_SILO_PORT"),
                    configuration.GetValue<int>("ORLEANS_GATEWAY_PORT")
                );
            }

            base.Build(siloBuilder, configuration);
        }
    }
}
