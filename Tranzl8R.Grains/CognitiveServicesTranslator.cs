using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orleans;
using Orleans.Placement;
using Orleans.Runtime;
using Orleans.Runtime.Placement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tranzl8R
{
    public class CognitiveServicesTranslator : Grain, ITranslator
    {
        static readonly string TranslationUrl = "https://api.cognitive.microsofttranslator.com/{0}?api-version=3.0";
        string _languageCodeBeingServed = String.Empty;

        public CognitiveServicesTranslator(IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            Configuration = configuration;
            HttpClientFactory = httpClientFactory;
        }

        public IConfiguration Configuration { get; }
        public IHttpClientFactory HttpClientFactory { get; }

        public async Task CheckIn(ITranslationServer languageServer)
        {
            _languageCodeBeingServed = this.GetGrainIdentity().PrimaryKeyString;
        }

        public async Task CheckOut(ITranslationServer languageServer)
        {
            // no work to do here yet - eventually dispose of the translator
        }

        public async Task<string> Translate(string originalPhrase, string originalLanguageCode = "en")
        {
            string endpoint = string.Format(TranslationUrl, "translate");
            string uri = string.Format(endpoint + "&from={0}&to={1}", originalLanguageCode, _languageCodeBeingServed);

            System.Object[] body = new System.Object[] { new { Text = originalPhrase } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = HttpClientFactory.CreateClient())
            using (var request = client.SetupTranslatorRequestFromConfiguration(Configuration, uri, requestBody))
            {
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<List<Dictionary<string, List<Dictionary<string, string>>>>>(responseBody);
                var translation = result[0]["translations"][0]["text"];
                return translation;
            }
        }
    }
}
