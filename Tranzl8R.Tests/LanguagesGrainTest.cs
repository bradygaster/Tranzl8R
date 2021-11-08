using Orleans.TestingHost;
using System;
using System.Linq;
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
            var grain = _cluster.GrainFactory.GetGrain<ITranslationServer>(Guid.Empty);
            var languages = await grain.GetAllLanguages();

            Assert.NotEmpty(languages);
        }

        [Fact]
        public async Task ShowsLanguagesThatHaveTranslators()
        {
            var translationServerGrain = _cluster.GrainFactory.GetGrain<ITranslationServer>(Guid.Empty);
            var languages = await translationServerGrain.GetAllLanguages();
            Assert.True(languages.First(_ => _.Code == "es").IsTranslatorReady == false);
            var spanishCode = "es";
            var spanishTranslator = _cluster.GrainFactory.GetGrain<ITranslator>(spanishCode);
            await spanishTranslator.CheckIn(translationServerGrain, spanishCode);
            languages = await translationServerGrain.GetAllLanguages();
            Assert.True(languages.First(_ => _.Code == "es").IsTranslatorReady == true);
        }

        //[Fact]
        //public async Task TranslatorsCanCheckIn()
        //{
        //    Assert.False(true);
        //}

        //[Fact]
        //public async Task TranslatorsActuallyTranslate()
        //{
        //    Assert.False(true);
        //}
    }
}