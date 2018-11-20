// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    public class QnADialog : Dialog
    {
        // See (https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-qna) for additional information.
        public const int QnaNumResults = 1;
        public const double QnaConfidenceThreshold = 0.5;

        public const string QnaConfiguration = "cafeFaqChitChat";

        public const string Name = "QnA";

        private readonly BotServices _services;
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        public QnADialog(BotServices services, IStatePropertyAccessor<UserProfile> userProfileAccessor, string dialogId = null)
            : base(Name)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _userProfileAccessor = userProfileAccessor ?? throw new ArgumentNullException(nameof(userProfileAccessor));
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Call QnA Maker and get results.
            var qnaResult = await _services.QnaServices[QnaConfiguration].GetAnswersAsync(dc.Context);
            if (qnaResult == null || qnaResult.Count() <= 0)
            {
                // No answer found.
                await dc.Context.SendActivityAsync("I'm still learning... Sorry, I do not know how to help you with that.");
                await dc.Context.SendActivityAsync($"Follow[this link](https://www.bing.com/search?q={dc.Context.Activity.Text}) to search the web!");
            }
            else
            {
                // Respond with QnA result.
                await dc.Context.SendActivityAsync(await UserSalutationAsync(dc.Context) + qnaResult[0].Answer);
            }

            return await dc.EndDialogAsync();
        }

        private async Task<string> UserSalutationAsync(ITurnContext context)
        {
            var salutation = string.Empty;
            var userProfile = await _userProfileAccessor.GetAsync(context, () => new UserProfile(string.Empty));
            if (userProfile == null && !string.IsNullOrWhiteSpace(userProfile.UserName))
            {
                var userName = userProfile.UserName;

                // See if we have user's name.
                string[] userSalutationList =
                {
                    string.Empty,
                    string.Empty,
                    $"Well... {userName}, ",
                    $"{userName}, ",
                };

                // Randomly include user's name in response so the reply in personalized.
                var rnd = new Random();
                var randomNumberIdx = (int)Math.Floor(rnd.NextDouble() * userSalutationList.Length);
                if (userSalutationList[randomNumberIdx] != null)
                {
                    salutation = userSalutationList[randomNumberIdx];
                }
            }

            return salutation;
        }
    }
}
