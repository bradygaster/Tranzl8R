namespace Tranzl8R
{
    [Serializable]
    public class LanguageServerState
    {
        public List<LanguageItem> AllLanguages { get; set; } = new List<LanguageItem>();
        public List<LanguageItem> LanguagesWithTranslators { get; set; } = new List<LanguageItem>();
    }
}
