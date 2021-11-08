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

        [Fact]
        public async Task TranslatorsCanCheckIn()
        {
            var translationServerGrain = _cluster.GrainFactory.GetGrain<ITranslationServer>(Guid.Empty);
            var languages = await translationServerGrain.GetAllLanguages();
            var langCode = "de";
            Assert.True(languages.First(_ => _.Code == langCode).IsTranslatorReady == false);
            var translator = _cluster.GrainFactory.GetGrain<ITranslator>(langCode);
            await translator.CheckIn(translationServerGrain, langCode);
            languages = await translationServerGrain.GetAllLanguages();
            Assert.True(languages.First(_ => _.Code == langCode).IsTranslatorReady == true);
        }

        [Fact]
        public async Task MultipleTranslatorsCanCheckInForRandomAvailableLanguages()
        {
            var countToAdd = 15;
            var translationServerGrain = _cluster.GrainFactory.GetGrain<ITranslationServer>(Guid.Empty);
            var languages = await translationServerGrain.GetAllLanguages();
            var totalLanguagesWithCheckedInTranslators = languages.Where(_ => _.IsTranslatorReady == true).Count();
            var randomAvailableLanguages = languages.Where(_ => _.IsTranslatorReady == false)
                .OrderBy(_ => new Random().Next())
                .Take(countToAdd)
                .ToList();
            foreach (var language in randomAvailableLanguages)
            {
                var translator = _cluster.GrainFactory.GetGrain<ITranslator>(language.Code);
                await translator.CheckIn(translationServerGrain, language.Code);
            }
            var totalWeShouldHaveNow = countToAdd + totalLanguagesWithCheckedInTranslators;
            var newTotalLanguagesWithCheckedInTranslators = (await translationServerGrain.GetAllLanguages())
                .Where(_ => _.IsTranslatorReady == true).Count();
            Assert.Equal(totalWeShouldHaveNow, newTotalLanguagesWithCheckedInTranslators);
        }

        //[Fact]
        //public async Task TranslatorsActuallyTranslate()
        //{
        //    Assert.False(true);
        //}
    }
}