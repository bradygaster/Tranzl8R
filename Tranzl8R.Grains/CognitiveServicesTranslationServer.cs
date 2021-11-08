using Microsoft.Extensions.Configuration;
using Orleans;

namespace Tranzl8R
{
    public class CognitiveServicesTranslationServer : Grain, ITranslationServer
    {
        public CognitiveServicesTranslationServer(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            Configuration = configuration;
            HttpClientFactory = httpClientFactory;
        }

        private const string LanguageListUrl = "/languages?api-version=3.0&scope=translation";
        public IConfiguration Configuration { get; }
        public IHttpClientFactory HttpClientFactory { get; }
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

        public Task ToggleLanguageActiveStatus(string language)
        {
            var isReady = Languages.First(_ => _.Code == language).IsTranslatorReady;
            Languages.First(_ => _.Code == language).IsTranslatorReady = !isReady;
            return Task.CompletedTask;
        }

        public Task ReceiveTranslatedString(TranslationResponse response)
        {
            return Task.CompletedTask;
        }
    }
}
