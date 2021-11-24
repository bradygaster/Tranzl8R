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
        public CognitiveServicesTranslator(IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            Configuration = configuration;
            HttpClientFactory = httpClientFactory;
        }

        public IConfiguration Configuration { get; }
        public IHttpClientFactory HttpClientFactory { get; }

        public async Task<string> Translate(string originalPhrase, string originalLanguageCode = "en")
        {
            using (var client = HttpClientFactory.CreateClient())
            using (var request = client.SetupTranslatorRequestFromConfiguration(Configuration, originalLanguageCode, this.GetGrainIdentity().PrimaryKeyString))
            {
                System.Object[] body = new System.Object[] { new { Text = originalPhrase } };
                var requestBody = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<List<Dictionary<string, List<Dictionary<string, string>>>>>(responseBody);
                var translation = result[0]["translations"][0]["text"];
                return translation;
            }
        }
    }
}
