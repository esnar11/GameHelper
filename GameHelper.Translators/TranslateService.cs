using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameHelper.Interfaces;

namespace GameHelper.Translators
{
/*
    public class TranslateService: ITranslateService
    {
        private readonly TranslationEngine _translationEngine;
        private readonly TranslatorLanguague _from;
        private readonly TranslatorLanguague _to;
        private readonly WebTranslator _webTranslator;

        public TranslateService()
        {
            var languagues = new List<TranslatorLanguague>
            {
                new TranslatorLanguague("English", "Russian", "RU"),
                new TranslatorLanguague("English", "Russian", "EN"),
                new TranslatorLanguague("English", "English", "EN"),
                new TranslatorLanguague("Auto", "Russian", "RU")
            };
            _translationEngine = new TranslationEngine(TranslationEngineName.GoogleTranslate, languagues, 1);
            _from = new TranslatorLanguague("English", "English", "EN");
            _to = new TranslatorLanguague("Russian", "Russian", "RU");
            _webTranslator = new WebTranslator(new Log());
            _webTranslator.LoadLanguages();
        }

        public async Task<string> Translate(string value, CancellationToken cancellationToken = default)
        {
            var result = await _webTranslator.TranslateAsync(value, _translationEngine, _from, _to);
            return result;
        }
    }

    internal class Log: ILog
    {
        public void WriteLog(string InputString, string memberName = "", int sourceLineNumber = 0)
        {
        }
    }
*/
}
