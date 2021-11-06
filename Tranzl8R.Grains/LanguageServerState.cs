using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tranzl8R
{
    [Serializable]
    public class LanguageServerState
    {
        public List<LanguageItem> AllLanguages { get; set; }
        public List<LanguageItem> LanguagesWithTranslators { get; set; }
    }
}
