using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tranzl8R
{
    internal class CognitiveServicesTranslator : Grain, ITranslator
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

        public async Task<string> Translate(ITranslationServer languageServer, string originalPhrase, string originalLanguageCode = "en")
        {
            var translatedPhrase = "[Translated Phrase Here]";
            await languageServer.ReceiveTranslatedString(new TranslationResponse(_languageCodeBeingServed,
                translatedPhrase, originalPhrase)
            );
            return translatedPhrase;
        }
    }
}
