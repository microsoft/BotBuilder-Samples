using Microsoft.Bot.Builder.AI.Translation;
using Microsoft.Bot.Builder.AI.Translation.PostProcessor;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace LuisBot.Controllers
{
    public static class TranslatorController
    {
        private static ITranslator _translator;
        private static string _userLanguage;
        private static string[] _nativeLanguages;
        private static string[] _supportedLanguages;

        static TranslatorController()
        {
            _translator = new Translator(WebConfigurationManager.AppSettings["translatorKey"], GetPatterns(), GetCusomDictionary());
            _nativeLanguages = GetNativeLanguages();
            _supportedLanguages = GetSupportedLanguages();
        }

        /// <summary>
        /// Check if user input is on the form "set my language to {languageId}
        /// </summary>
        /// <param name="activity"></param>
        /// <returns>A task that represents the check operation.
        /// The task result is true if the user changed his language.</returns>
        public static async Task<bool> IsUserChangedLangauage(Activity activity)
        {
            bool changeLang = false;//logic implemented by developper to make a signal for language changing 
            
            //use a specific message from user to change language
            var messageActivity = activity.Text;
            if (messageActivity.ToLower().StartsWith("set my language to"))
            {
                changeLang = true;
            }
            if (changeLang)
            {
                string reply = "";
                var newLang = messageActivity.ToLower().Replace("set my language to", "").Trim();
                if (!string.IsNullOrWhiteSpace(newLang) && IsSupportedLanguage(newLang))
                {
                    SetLanguage(newLang);
                    reply = $@"Changing your language to {newLang}";
                }
                else
                {
                    reply = $@"{newLang} is not a supported language.";
                }
                await SendReply(activity, reply).ConfigureAwait(false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Translates the activity text of the user from sourceLanguage to targetLanguage
        /// </summary>
        /// <param name="activity"></param>
        /// <returns>A task that represents the asyncronous translation operation.</returns>
        public static async Task TranslateActivityTextAsync(Activity activity)
        {
            string sourceLanguage;
            if (string.IsNullOrEmpty(_userLanguage))
                sourceLanguage = await DetectLanguageAsync(activity.Text).ConfigureAwait(false);
            else
                sourceLanguage = _userLanguage;
            ITranslatedDocument translatedDocument = await TranslateAsync(activity.Text, sourceLanguage, _nativeLanguages[0]).ConfigureAwait(false);
            activity.Text = translatedDocument.GetTranslatedMessage();
        }

        private static void SetLanguage(string newLang)
        {
            _userLanguage = newLang;
        }

        private static bool IsSupportedLanguage(string language)
        {
            return _supportedLanguages.Contains(language);
        }


        private static async Task<string> DetectLanguageAsync(string textToDetect)
        {
            return await _translator.DetectAsync(textToDetect).ConfigureAwait(false);
        }

        private static async Task<ITranslatedDocument> TranslateAsync(string textToTrasnlate, string sourceLanguage, string targetLanguage)
        {
            return await _translator.TranslateAsync(textToTrasnlate, sourceLanguage, targetLanguage).ConfigureAwait(false);
        }

        private static async Task<List<ITranslatedDocument>> TranslateArrayAsync(string[] textToTrasnlate, string sourceLanguage, string targetLanguage)
        {
            return await _translator.TranslateArrayAsync(textToTrasnlate, sourceLanguage, targetLanguage).ConfigureAwait(false);
        }

        private static Dictionary<string, List<string>> GetPatterns()
        {
            string path = GetPath("patternsPath");
            var json = File.ReadAllText(path);
            var patterns = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            return patterns;
        }

        private static CustomDictionary GetCusomDictionary()
        {
            string path = GetPath("dictionaryPath");
            var json = File.ReadAllText(path);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            CustomDictionary customDictionary = new CustomDictionary();
            foreach (KeyValuePair<string, Dictionary<string, string>> lang in dictionary)
            {
                customDictionary.AddNewLanguageDictionary(lang.Key, lang.Value);
            }
            return customDictionary;
        }

        private static string[] GetNativeLanguages()
        {
            string path = GetPath("nativeLanguagesPath");
            var json = File.ReadAllText(path);
            var languages = JsonConvert.DeserializeObject<string[]>(json);
            return languages;
        }

        private static string[] GetSupportedLanguages()
        {
            var curr = WebConfigurationManager.AppSettings["supportedLanguagesPath"];
            string path = HttpContext.Current.Server.MapPath(curr);
            var json = File.ReadAllText(path);
            var languages = JsonConvert.DeserializeObject<string[]>(json);
            return languages;
        }

        private static string GetPath(string tag)
        {
            return HttpContext.Current.Server.MapPath(WebConfigurationManager.AppSettings[tag]);
        }

        private static async Task SendReply(Activity activity, string reply)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(reply)).ConfigureAwait(false);
        }

    }
}