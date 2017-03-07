using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using NewsieBot.Models.News;
using NewsieBot.Services;
using NewsieBot.Utilities;
using NewsieBot.Utilities.CardGenerators;

namespace NewsieBot.Handlers
{
    internal sealed class SummaryRegexHandler : IRegexHandler
    {
        private readonly ICacheManager cache;
        private readonly ISummarizeService summaryService;
        private readonly IBotToUser botToUser;

        public SummaryRegexHandler(IBotToUser botToUser, ICacheManager cache, ISummarizeService summaryService)
        {
            SetField.NotNull(out this.cache, nameof(cache), cache);
            SetField.NotNull(out this.summaryService, nameof(summaryService), summaryService);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
        }

        public async Task Respond(IMessageActivity activity, Match result)
        {
            string articleUrl = TryParseSummaryUrl(activity.Text);

            if (!string.IsNullOrEmpty(articleUrl))
            {
                await this.botToUser.PostAsync(string.Format(Strings.SummaryWaitMessage));
                
                Value newsResult;
                var hasNewsResult = this.cache.TryRead(articleUrl, out newsResult);

                if (hasNewsResult)
                {
                    var newsieResult = new NewsieNewsResult();

                    // Return news headline and image 
                    var newsFirstMessage = this.botToUser.MakeMessage();

                    newsieResult.ShortenedUrl = articleUrl;
                    NewsIntentHandler.PrepareNewsieResultHelper(newsResult, newsieResult);

                    newsFirstMessage.Attachments = new List<Attachment>
                    {
                        NewsCardGenerator.GetNewsArticleHeadlineImageCard(newsieResult, activity.ChannelId)
                    };

                    await this.botToUser.PostAsync(newsFirstMessage);
                }

                var bingSummary = await this.summaryService.GetSummary(articleUrl);

                if (bingSummary?.Data != null && bingSummary.Data.Length != 0)
                {
                    var newsSummaryMessage = this.botToUser.MakeMessage();

                    newsSummaryMessage.Attachments.Add(CardGenerator.GetHeroCard(Strings.SummaryString));

                    await this.botToUser.PostAsync(Strings.SummaryString);

                    foreach (var t in bingSummary.Data)
                    {
                        newsSummaryMessage = this.botToUser.MakeMessage();

                        newsSummaryMessage.Attachments.Add(CardGenerator.GetHeroCard(text: t.Text));

                        await this.botToUser.PostAsync(newsSummaryMessage);
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
            bool result = Uri.TryCreate(text.Substring("summary".Length), UriKind.Absolute, out uriResult)
                && uriResult.Scheme == Uri.UriSchemeHttp;

            if (result && uriResult.IsAbsoluteUri)
            {
                return uriResult.AbsoluteUri;
            }

            return string.Empty;
        }
    }
}