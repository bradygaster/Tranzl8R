using Microsoft.Extensions.Configuration;
using Orleans;

namespace Tranzl8R
{
    public class CognitiveServicesAvailableLanguages : Grain, ILanguagesGrain
    {
        public CognitiveServicesAvailableLanguages(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            Configuration = configuration;
            HttpClientFactory = httpClientFactory;
        }

        public IConfiguration Configuration { get; }
        public IHttpClientFactory HttpClientFactory { get; }

        public async Task<List<LanguageItem>> GetAllLanguages()
        {
            using (var client = HttpClientFactory.CreateClient())
            using (var request = HttpClientFactory.CreateClient().SetupTranslatorRequestFromConfiguration(Configuration, "/languages?api-version=3.0&scope=translation"))
            {
                request.Method = HttpMethod.Get;
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                return await response.ParseLanguagesApiCall();
            }
        }
    }
}
