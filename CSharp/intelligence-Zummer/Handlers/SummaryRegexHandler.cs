using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Zummer.Services;
using Zummer.Utilities.CardGenerators;

namespace Zummer.Handlers
{
    internal sealed class SummaryRegexHandler : IRegexHandler
    {
        private readonly ISummarizeService summaryService;
        private readonly IBotToUser botToUser;

        public SummaryRegexHandler(IBotToUser botToUser, ISummarizeService summaryService)
        {
            SetField.NotNull(out this.summaryService, nameof(summaryService), summaryService);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
        }

        public async Task Respond(IMessageActivity activity, Match result)
        {
            if (result != null && result.Groups.Count == 3 && !string.IsNullOrEmpty(result.Groups[2].Value))
            {
                string articleUrl = TryParseSummaryUrl(result.Groups[2].Value);

                var fullArticleMessage = this.botToUser.MakeMessage();

                fullArticleMessage.Attachments.Add(CardGenerator.GetHeroCard(Strings.SummaryWaitMessage, cardActions: new List<CardAction>
                {
                    new CardAction
                    {
                        Title = Strings.FullArticleString,
                        Type = ActionTypes.OpenUrl,
                        Value = articleUrl
                    }
                }));

                await this.botToUser.PostAsync(fullArticleMessage);
                
                var bingSummary = await this.summaryService.GetSummary(articleUrl);

                if (bingSummary?.Data != null && bingSummary.Data.Length != 0)
                {
                    await this.botToUser.PostAsync(Strings.SummaryString);

                    foreach (var datum in bingSummary.Data)
                    {
                        var summaryMessage = this.botToUser.MakeMessage();

                        summaryMessage.Attachments.Add(CardGenerator.GetHeroCard(text: datum.Text));

                        await this.botToUser.PostAsync(summaryMessage);
                    }
                }
                else
                {
                    await this.botToUser.PostAsync(string.Format(Strings.SummaryErrorMessage));
                }
            }
            else
            {
                await this.botToUser.PostAsync(string.Format(Strings.SummaryErrorMessage));
            }
        }

        private static string TryParseSummaryUrl(string text)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(text, UriKind.Absolute, out uriResult);

            if (result && uriResult.IsAbsoluteUri)
            {
                return uriResult.AbsoluteUri;
            }

            return string.Empty;
        }
    }
}