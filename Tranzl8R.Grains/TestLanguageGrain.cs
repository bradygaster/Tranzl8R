using Orleans;

namespace Tranzl8R
{
    public class TestLanguageGrain : Grain, ILanguagesGrain
    {
        public Task<string> GetAvailableLanguages()
        {
            return Task.FromResult("Klingon");
        }
    }
}
