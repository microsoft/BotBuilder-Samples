using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Newsie.Models.News;
using Newsie.Services;
using Newsie.Utilities;
using Newsie.Utilities.CardGenerators;

namespace Newsie.Handlers
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
                await this.botToUser.PostAsync(string.Format(Strings.SummaryWaitMessage, Emojis.Rocket));
                
                Value newsResult;
                var hasNewsResult = this.cache.TryRead(articleUrl, out newsResult);

                if (hasNewsResult)
                {
                    var newsieResult = new NewsieNewsResult();

                    // Return news headline and image 
                    var newsHeadline = this.botToUser.MakeMessage();
                    NewsIntentHandler.PrepareNewsieResultHelper(newsResult, newsieResult);

                    newsHeadline.Attachments = new List<Attachment>
                    {
                        NewsCardGenerator.GetNewsArticleHeadlineCard(newsieResult),
                        NewsCardGenerator.GetNewsArticleHeadlineImage(newsieResult)
                    };

                    await this.botToUser.PostAsync(newsHeadline);
                }

                var bingSummary = await this.summaryService.GetSummary(articleUrl);

                if (bingSummary?.Data != null && bingSummary.Data.Length != 0)
                {
                    foreach (var t in bingSummary.Data)
                    {
                        await this.botToUser.PostAsync(t.Text);
                    }
                }
                else
                {
                    await this.botToUser.PostAsync(string.Format(Strings.SummaryErrorMessage, Emojis.Flushed));
                }
            }
            else
            {
                await this.botToUser.PostAsync(string.Format(Strings.SummaryErrorMessage, Emojis.Flushed));
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