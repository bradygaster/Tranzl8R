using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orleans;

namespace Tranzl8R
{
    // todo: clean this implementation up as it is quite brute-force atm
    public class CognitiveServicesAvailableLanguages : Grain, ILanguagesGrain
    {
        public CognitiveServicesAvailableLanguages(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public async Task<List<LanguageItem>> GetAvailableLanguages()
        {
            var ret = new List<LanguageItem>();
            string route = "/languages?api-version=3.0&scope=translation";
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(Configuration["AZURE_TRANSLATOR_ENDPOINT"] + route);
                request.Headers.Add("Ocp-Apim-Subscription-Key", Configuration["AZURE_TRANSLATOR_SUBSCRIPTION_KEY"]);
                request.Headers.Add("Ocp-Apim-Subscription-Region", Configuration["AZURE_TRANSLATOR_LOCATION"]);
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);
                var languages = result["translation"];
                var languageCodes = languages.Keys.ToArray();
                foreach (var kv in languages)
                {
                    ret.Add(new LanguageItem(kv.Value["name"], kv.Key));
                }
            }
            return ret;
        }
    }
}
