using Newtonsoft.Json;

namespace Tranzl8R
{
    internal static class TranslatorApiResponseExtensions
    {
        internal static async Task<List<LanguageItem>> ParseLanguagesApiCall(this HttpResponseMessage response)
        {
            var ret = new List<LanguageItem>();
            string json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);
            var languages = result["translation"];
            var languageCodes = languages.Keys.ToArray();
            foreach (var kv in languages)
            {
                ret.Add(new LanguageItem(kv.Value["name"], kv.Key));
            }
            return ret;
        }
    }
}
