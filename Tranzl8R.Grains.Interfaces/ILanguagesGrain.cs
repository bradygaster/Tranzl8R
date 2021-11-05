using Orleans;
using System.Threading.Tasks;

namespace Tranzl8R
{
    public interface ILanguagesGrain : IGrainWithGuidKey
    {
        Task<string> GetAvailableLanguages();
    }
}
