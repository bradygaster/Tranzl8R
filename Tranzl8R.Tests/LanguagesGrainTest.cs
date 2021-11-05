using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tranzl8R.Tests
{
    public class LanguagesGrainTest : IClassFixture<ClusterFixture>
    {
        private readonly TestCluster _cluster;
        public LanguagesGrainTest(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public async Task ProvidesAvailableLanguagesCorrectly()
        {
            var grain = _cluster.GrainFactory.GetGrain<ILanguagesGrain>(Guid.Empty);
            var language = await grain.GetAvailableLanguages();

            Assert.Equal("Klingon", language);
        }
    }
}