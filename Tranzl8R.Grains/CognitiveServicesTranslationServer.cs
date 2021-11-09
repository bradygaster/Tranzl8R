using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Orleans;

namespace Tranzl8R
{
    public class CognitiveServicesTranslationServer : Grain, ITranslationServer
    {
        public CognitiveServicesTranslationServer(IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IGrainFactory grainFactory)
        {
            Configuration = configuration;
            HttpClientFactory = httpClientFactory;
            GrainFactory = grainFactory;
        }

        private const string LanguageListUrl = "/languages?api-version=3.0&scope=translation";
        public IConfiguration Configuration { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public IGrainFactory GrainFactory { get; }
        public List<LanguageItem> Languages { get; internal set; } = new List<LanguageItem>();

        public async Task<List<LanguageItem>> GetAllLanguages()
        {
            if(!Languages.Any())
            {
                using (var client = HttpClientFactory.CreateClient())
                using (var request = client.SetupTranslatorRequestFromConfiguration(Configuration, LanguageListUrl))
                {
                    request.Method = HttpMethod.Get;
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    Languages = await response.ParseLanguagesApiCall();
                }
            }

            return Languages;
        }

        public async Task ToggleLanguageActiveStatus(string language)
        {
            var isReady = !(Languages.First(_ => _.Code == language).IsTranslatorReady);
            var translator = GrainFactory.GetGrain<ITranslator>(language);

            if (isReady)
            {
                await translator.CheckIn(this);
            }
            else
            {
                await translator.CheckOut(this);
            }

            Languages.First(_ => _.Code == language).IsTranslatorReady = isReady;
        }

        public async Task<List<TranslationResponse>> Translate(string phrase, string phraseLanguage = "en")
        {
            var languagesWithTranslators = Languages.Where(_ => _.IsTranslatorReady).ToList();
            var result = new List<TranslationResponse>();
            var taskList = new List<Task>();

            foreach (var language in languagesWithTranslators)
            {
                taskList.Add(Task.Run(async () =>
                {
                    var translator = GrainFactory.GetGrain<ITranslator>(language.Code);
                    var translatedPhrase = await translator.Translate(phrase);
                    result.Add(new TranslationResponse(language.Code, translatedPhrase, phrase));
                }));
            }

            Task.WhenAll(taskList).Wait();

            return result;
        }
    }
}
