using Microsoft.Extensions.Configuration;
using Orleans.Configuration;
using System.Net;
using System.Net.Sockets;

namespace Orleans.Hosting
{
    public class ConfiguredEndpointsBuilder : AzureSiloBuilder
    {
        public override void Build(ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(configuration.GetValue<string>("ORLEANS_SILO_PORT")) &&
                !string.IsNullOrEmpty(configuration.GetValue<string>("ORLEANS_GATEWAY_PORT")))
            {
                siloBuilder.Configure<EndpointOptions>(options =>
                {
                    options.SiloPort = configuration.GetValue<int>("ORLEANS_SILO_PORT");
                    options.GatewayPort = configuration.GetValue<int>("ORLEANS_GATEWAY_PORT");

                    var siloHostEntry = Dns.GetHostEntry(Environment.MachineName);
                    options.AdvertisedIPAddress = siloHostEntry.AddressList[0];
                });
            }

            base.Build(siloBuilder, configuration);
        }
    }
}
