using Microsoft.Extensions.Configuration;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        private static HttpRequestMessage BaseSetup(this HttpClient client,
            IConfiguration configuration)
        {
            var request = new HttpRequestMessage();
            request.Headers.Add("Ocp-Apim-Subscription-Key", configuration["AZURE_TRANSLATOR_SUBSCRIPTION_KEY"]);
            request.Headers.Add("Ocp-Apim-Subscription-Region", configuration["AZURE_TRANSLATOR_LOCATION"]);
            return request;
        }

        public static HttpRequestMessage SetupLanguageListRequestFromConfiguration(this HttpClient client,
            IConfiguration configuration)
        {
            var request = client.BaseSetup(configuration);
            var route = "/languages?api-version=3.0&scope=translation";
            request.RequestUri = new Uri(configuration["AZURE_TRANSLATOR_ENDPOINT"] + route);
            return request;
        }

        public static HttpRequestMessage SetupTranslatorRequestFromConfiguration(this HttpClient client,
            IConfiguration configuration,
            string originalLanguage,
            string destinationLanguage)
        {
            var request = client.BaseSetup(configuration);
            var endpoint = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0";
            var uri = string.Format(endpoint + "&from={0}&to={1}", originalLanguage, destinationLanguage);
            request.RequestUri = new Uri(uri);
            request.Method = HttpMethod.Post;
            request.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());
            return request;
        }
    }
}
