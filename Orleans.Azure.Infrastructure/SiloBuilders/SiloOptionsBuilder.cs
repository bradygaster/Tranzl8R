using Microsoft.Extensions.Configuration;
using Orleans.Configuration;

namespace Orleans.Hosting
{
    public class SiloOptionsBuilder : AzureSiloBuilder
    {
        public override void Build(ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            var siloName = string.IsNullOrEmpty(
                configuration.GetValue<string>("ORLEANS_SILO_NAME")) 
                    ? "Silo" 
                    : configuration.GetValue<string>("ORLEANS_SILO_NAME"
                );
            siloBuilder.Configure<SiloOptions>(options => options.SiloName = siloName);

            base.Build(siloBuilder, configuration);
        }
    }
}
