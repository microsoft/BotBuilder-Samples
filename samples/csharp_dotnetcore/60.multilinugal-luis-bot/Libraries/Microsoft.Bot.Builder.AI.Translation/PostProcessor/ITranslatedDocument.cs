using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Bot.Builder.AI.Translation.PostProcessor
{
    /// <summary>
    /// Define Transltaed Document.
    /// </summary>
    public interface ITranslatedDocument
    {
        void SetSourceMessage(string sourceMessage);

        string GetSourceMessage();

        void SetTranslatedMessage(string translatedMessage);

        string GetTranslatedMessage();

        void SetRawAlignment(string alignment);

        string GetRawAlignment();

        void SetIndexedAlignment(Dictionary<int, int> indexedAlignment);

        Dictionary<int, int> GetIndexedAlignment();

        void SetSourceTokens(string[] sourceTokens);

        string[] GetSourceTokens();

        void SetTranslatedTokens(string[] translatedTokens);

        string[] GetTranslatedTokens();

        void SetLiteranlNoTranslatePhrases(HashSet<string> literanlNoTranslatePhrases);

        HashSet<string> GetLiteranlNoTranslatePhrases();
    }
}
