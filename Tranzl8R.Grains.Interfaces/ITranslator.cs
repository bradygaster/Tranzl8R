using Orleans;
using System.Threading.Tasks;

namespace Tranzl8R
{
    public interface ITranslator : IGrainWithStringKey
    {
        Task CheckIn(ITranslationServer languageServer, string languageCode);
        Task CheckOut(ITranslationServer languageServer);
        Task<string> Translate(ITranslationServer languageServer, string originalPhrase, string originalLanguageCode = "en");
    }
}
