using Orleans.TestingHost;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Xunit.Abstractions;

namespace Tranzl8R.Tests
{
    public class LanguagesGrainTest : IClassFixture<ClusterFixture>
    {
        private readonly TestCluster _cluster;
        private readonly ITestOutputHelper _output;

        public LanguagesGrainTest(ClusterFixture fixture, ITestOutputHelper output)
        {
            _cluster = fixture.Cluster;
            _output = output;
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

        [Fact]
        public async Task TranslatorsPeformTranslation()
        {
            var translationServerGrain = _cluster.GrainFactory.GetGrain<ITranslationServer>(Guid.Empty);
            var languages = await translationServerGrain.GetAllLanguages();
            var totalLanguagesWithCheckedInTranslators = languages.Where(_ => _.IsTranslatorReady == true).Count();

            languages.Where(_ => _.IsTranslatorReady == false)
                    .OrderBy(_ => new Random().Next())
                    .Take(5)
                    .ToList()
                    .ForEach(async (language) =>
                    {
                        var translator = _cluster.GrainFactory.GetGrain<ITranslator>(language.Code);
                        await translator.CheckIn(translationServerGrain, language.Code);
                    });

            languages = await translationServerGrain.GetAllLanguages();
            totalLanguagesWithCheckedInTranslators = languages.Where(_ => _.IsTranslatorReady == true).Count();

            var results = await translationServerGrain.Translate("Hello World");
            results.ForEach(r => _output.WriteLine(r.Translation));

            Assert.Equal(totalLanguagesWithCheckedInTranslators, results.Count);
        }
    }
}