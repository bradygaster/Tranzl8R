using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Tranzl8R.Infrastructure
{
    public class MonitoredClusterMemberInitializer : ITelemetryInitializer
    {
        public MonitoredClusterMemberInitializer(string nodeName)
        {
            Name = nodeName;
        }

        public string Name { get; set; }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = Name;
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    using Tranzl8R.Infrastructure;
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationInsightsMonitoring(this IServiceCollection services, string nodeName)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<ITelemetryInitializer>(new MonitoredClusterMemberInitializer(nodeName));
        }
    }
}