using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tranzl8R
{
    public interface ILanguagesGrain : IGrainWithGuidKey
    {
        Task<List<LanguageItem>> GetAllLanguages();
    }

    public record LanguageItem(string Name, 
        string Code, 
        bool IsTranslatorReady = false);
}
