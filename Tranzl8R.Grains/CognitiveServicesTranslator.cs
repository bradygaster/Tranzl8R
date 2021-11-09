using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tranzl8R
{
    public class CognitiveServicesTranslator : Grain, ITranslator
    {
        string _languageCodeBeingServed = String.Empty;

        public async Task CheckIn(ITranslationServer languageServer, string languageCode)
        {
            _languageCodeBeingServed = languageCode;
            await languageServer.ToggleLanguageActiveStatus(languageCode);
        }

        public async Task CheckOut(ITranslationServer languageServer)
        {
            await languageServer.ToggleLanguageActiveStatus(_languageCodeBeingServed);
        }

        public async Task<string> Translate(string originalPhrase, string originalLanguageCode = "en")
        {
            var translatedPhrase = $"[{originalPhrase} translated to {_languageCodeBeingServed} here]";
            return translatedPhrase;
        }
    }
}
