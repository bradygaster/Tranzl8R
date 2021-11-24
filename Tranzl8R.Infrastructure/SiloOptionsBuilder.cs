using Microsoft.Extensions.Configuration;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Tranzl8R.Infrastructure
{
    public class SiloOptionsBuilder : AzureSiloBuilder
    {
        public override void Build(ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            if (configuration.GetValue<string>("ORLEANS_SILO_NAME") != null)
            {
                var siloName = configuration.GetValue<string>("ORLEANS_SILO_NAME");
                siloBuilder.Configure<SiloOptions>(options => options.SiloName = string.IsNullOrEmpty(siloName) ? "Silo" : siloName);
            }

            base.Build(siloBuilder, configuration);
        }
    }
}
