using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Hosting
{
    public class LocalhostSiloClientBuilder : AzureSiloClientBuilder
    {
        public override void Build(IClientBuilder clientBuilder, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING")) &&
                string.IsNullOrEmpty(configuration.GetValue<string>("ORLEANS_AZURE_STORAGE_CONNECTION_STRING")))
            // check for other clustering configurations, and if none are found...)
            {
                clientBuilder.UseLocalhostClustering();
            }

            base.Build(clientBuilder, configuration);
        }
    }
}
