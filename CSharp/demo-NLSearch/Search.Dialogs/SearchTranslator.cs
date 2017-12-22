using Microsoft.Bot.Builder.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Search.Utilities;
using System.Globalization;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Dialogs;


// Notes:
// --In order to handle markup, I needed to transform in/out.
// --Seems like I need to escape things like >, but when I do it messes up translation in SMT, but works in NN.
// --I needed to add "rooms" as a synonynm for bedroom for French
// --Capitalization is not handled reliably, i.e. "filter" becomes "Filter"
// --We need to have no translate on input (your name) and output (your name)
namespace Search.Dialogs
{
    public class SearchTranslator : IActivityLogger
    {
        public string BotLanguage
        {
            get;
        }

        public string UserLanguage
        {
            get; set;
        }

        private readonly ConversationReference Conversation;
        private Translator Translation;
        private readonly IBotData botData;

        public SearchTranslator(ConversationReference conversation, IBotData botData, string botLanguge, string translationKey)
        {
            this.Conversation = conversation;
            this.BotLanguage = botLanguge;
            this.botData = botData;
            this.Translation = new Translator(translationKey);
        }

        private void TranslateButtons(IList<CardAction> buttons, Func<string, string> transform)
        {
            foreach (var button in buttons)
            {
                button.Title = transform(button.Title);
                button.Text = transform(button.Text);
                if (button.Type == "imBack")
                {
                    button.Value = transform((string)button.Value);
                }
            }
        }

        private void TranslateMessage(IMessageActivity message, Func<string, string> translate)
        {
            Func<string, string> transform = (text) => TranslateText(text, translate);
            message.Text = transform(message.Text);
            if (message.SuggestedActions != null)
            {
                foreach (var action in message.SuggestedActions.Actions)
                {
                    action.Title = transform(action.Title);
                    if (action.Type == "imBack")
                    {
                        action.Value = transform((string)action.Value);
                    }
                }
            }
            if (message.Attachments != null)
            {
                foreach (var attachment in message.Attachments)
                {
                    // TODO: Lots more card types
                    if (attachment.ContentType == "application/vnd.microsoft.card.thumbnail")
                    {
                        var thumbnail = attachment.Content as ThumbnailCard;
                        thumbnail.Title = transform(thumbnail.Title);
                        thumbnail.Subtitle = transform(thumbnail.Subtitle);
                        thumbnail.Text = transform(thumbnail.Text);
                        TranslateButtons(thumbnail.Buttons, transform);
                    }
                }
            }
        }

        private string TranslateText(string text, Func<string, string> transform)
        {
            string result = null;
            if (text != null)
            {
                var builder = new StringBuilder();
                var lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                bool first = true;
                foreach (var line in lines)
                {
                    if (!first)
                    {
                        builder.AppendLine();
                    }
                    else
                    {
                        first = false;
                    }
                    int start = 0;
                    while (start < line.Length && char.IsWhiteSpace(line[start]))
                    {
                        ++start;
                    }
                    if (start < line.Length && (line[start] == '*' || line[start] == '+' || line[start] == '-'))
                    {
                        ++start;
                        if (start < line.Length && char.IsWhiteSpace(line[start]))
                        {
                            ++start;
                            builder.Append(line.Substring(0, start));
                        }
                        else
                        {
                            start = 0;
                        }
                    }
                    var translation = transform(line.Substring(start));
                    int i = 0;
                    while (i < translation.Length)
                    {
                        var ch = translation[i];
                        char scan = '\0';
                        switch (ch)
                        {
                            case '*': scan = '*'; break;
                            case '_': scan = '_'; break;
                            default:
                                builder.Append(ch);
                                ++i;
                                break;
                        }
                        if (scan != '\0')
                        {
                            while (i < translation.Length && translation[i] == '*')
                            {
                                builder.Append(translation[i++]);
                            }
                            while (i < translation.Length && char.IsWhiteSpace(translation[i])) ++i;
                            while (i < translation.Length && translation[i] != scan)
                            {
                                builder.Append(translation[i++]);
                            }
                            var j = i - 1;
                            while (j > 0 && char.IsWhiteSpace(translation[j]))
                            {
                                --j;
                            }
                            var spaces = i - 1 - j;
                            builder.Remove(builder.Length - spaces, spaces);
                            while (i < translation.Length && translation[i] == scan)
                            {
                                builder.Append(translation[i++]);
                            }
                        }
                    }
                }
                result = builder.ToString();
            }
            return result;
        }

        public async Task LogAsync(IActivity activity)
        {
            this.UserLanguage = this.botData.UserData.GetValueOrDefault<string>("UserLanguage");

            var message = activity.AsMessageActivity();
            dynamic dmessage = activity;
            dmessage.Properties["OriginalText"] = message.Text;
            var userLanguage = this.UserLanguage ?? message?.Locale;
            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (userLanguage != this.BotLanguage)
                {
                    if (!message.Text.StartsWith("#"))
                    {
                        var fromLanguage = userLanguage;
                        var toLanguage = this.BotLanguage;
                        if (message.From.Id == this.Conversation.Bot.Id)
                        {
                            fromLanguage = this.BotLanguage;
                            toLanguage = userLanguage;
                        }

                        var strings = new List<string>();
                        TranslateMessage(message, (string input) =>
                        {
                            if (!string.IsNullOrWhiteSpace(input))
                            {
                                strings.Add(input);
                            }
                            return input;
                        });

                        var translations = await this.Translation.Translate(fromLanguage, toLanguage, strings.ToArray());

                        var i = 0;
                        TranslateMessage(message, (string input) =>
                        {
                            var translation = input;
                            if (!string.IsNullOrWhiteSpace(input))
                            {
                                translation = translations.Translations[i++];
                            }
                            return translation;
                        });
                    }
                } 
                else
                {
                    message.Text = Translator.RemoveLiteral(message.Text);
                }
            }
        }
    }
}
