using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tranzl8R
{
    public interface ITranslationServer : IGrainWithGuidKey
    {
        Task<List<LanguageItem>> GetAllLanguages();
        Task ToggleLanguageActiveStatus(string language);
        Task ReceiveTranslatedString(TranslationResponse response);
    }

    public record LanguageItem(string Name, string Code)
    {
        public bool IsTranslatorReady { get; set; }
    }

    public record TranslationResponse(string Language, string Translation, string OriginalPhrase);
}
