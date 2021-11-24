using Microsoft.Extensions.Configuration;
using Orleans.Hosting;
using System.Net;

namespace Tranzl8R.Infrastructure
{
    public class AzureWebAppSiloBuilder : AzureSiloBuilder
    {
        public override void Build(ISiloBuilder siloBuilder, IConfiguration configuration)
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

            base.Build(siloBuilder, configuration);
        }
    }
}
