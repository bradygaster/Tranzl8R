using Microsoft.Extensions.Configuration;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

namespace Tranzl8R.Infrastructure
{
    public abstract class AzureSiloBuilder
    {
        private AzureSiloBuilder? _successor;

        public void SetNextBuilder(AzureSiloBuilder successor)
        {
            _successor = successor;
        }

        public virtual void Build(ISiloBuilder siloBuilder, IConfiguration configuration)
        {
            if(_successor != null)
            {
                _successor.Build(siloBuilder, configuration);
            }
        }
    }
}
