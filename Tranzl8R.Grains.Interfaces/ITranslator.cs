using Orleans;
using System.Threading.Tasks;

namespace Tranzl8R
{
    public interface ITranslator : IGrainWithStringKey
    {
        Task<string> Translate(string originalPhrase, string originalLanguageCode = "en");
    }
}
