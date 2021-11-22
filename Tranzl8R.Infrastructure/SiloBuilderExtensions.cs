using Microsoft.Extensions.Configuration;
using System.Net;

namespace Orleans.Hosting
{
    public static class SiloBuilderExtensions
    {
        public static ISiloBuilder ConfigureEndpointsForAzureAppService(this ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            IPAddress endpointAddress = IPAddress.Parse(configuration.GetValue<string>("WEBSITE_PRIVATE_IP"));

            var strPorts = configuration.GetValue<string>("WEBSITE_PRIVATE_PORTS").Split(',');

            if (strPorts.Length < 2) throw new Exception("Insufficient private ports configured.");

            int siloPort = int.Parse(strPorts[0]);
            int gatewayPort = int.Parse(strPorts[1]);

            siloBuilder.ConfigureEndpoints(endpointAddress, siloPort, gatewayPort);

            return siloBuilder;
        }
    }
}
