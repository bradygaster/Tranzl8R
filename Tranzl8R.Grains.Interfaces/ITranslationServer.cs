using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tranzl8R
{
    public interface ITranslationServer : IGrainWithGuidKey
    {
        Task<List<LanguageItem>> GetAllLanguages();
        Task ToggleLanguageActiveStatus(string language);
    }

    public record LanguageItem(string Name, string Code)
    {
        public bool IsTranslatorReady { get; set; }
    }
}
