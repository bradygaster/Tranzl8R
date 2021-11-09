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

        public async Task CheckIn(ITranslationServer languageServer)
        {
            _languageCodeBeingServed = this.GetGrainIdentity().PrimaryKeyString;
        }

        public async Task CheckOut(ITranslationServer languageServer)
        {
            // no work to do here yet - eventually dispose of the translator
        }

        public async Task<string> Translate(string originalPhrase, string originalLanguageCode = "en")
        {
            var translatedPhrase = $"[{originalPhrase} translated to {_languageCodeBeingServed} here]";
            return translatedPhrase;
        }
    }
}
