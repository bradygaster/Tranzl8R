using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static HttpRequestMessage SetupTranslatorRequestFromConfiguration(this HttpClient client,
            IConfiguration configuration,
            string route)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(configuration["AZURE_TRANSLATOR_ENDPOINT"] + route);
            request.Headers.Add("Ocp-Apim-Subscription-Key", configuration["AZURE_TRANSLATOR_SUBSCRIPTION_KEY"]);
            request.Headers.Add("Ocp-Apim-Subscription-Region", configuration["AZURE_TRANSLATOR_LOCATION"]);
            return request;
        }
    }
}
