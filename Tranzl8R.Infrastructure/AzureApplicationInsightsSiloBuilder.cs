﻿using Microsoft.Extensions.Configuration;
using Orleans.Hosting;

namespace Tranzl8R.Infrastructure
{
    public class AzureApplicationInsightsSiloBuilder : AzureSiloBuilder
    {
        public override void Build(ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY")))
            {
                var azureMonitoringInstrumentationKey = configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");
                siloBuilder
                    .AddApplicationInsightsTelemetryConsumer(configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY"));
            }

            base.Build(siloBuilder, configuration);
        }
    }
}
