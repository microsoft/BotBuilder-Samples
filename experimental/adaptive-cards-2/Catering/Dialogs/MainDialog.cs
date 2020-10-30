// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catering.Cards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Catering.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger Logger;
        protected readonly IConfiguration Configuration;
        protected readonly CateringDb _cateringDb;

        public MainDialog(IConfiguration configuration, UserState userState, CateringDb cateringDb, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Configuration = configuration;
            Logger = logger;
            _cateringDb = cateringDb;
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext dialogContext, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dialogContext.Context.Activity.Type == ActivityTypes.Message)
            {
                var interupResult = await InterruptAsync(dialogContext, cancellationToken);
                if (interupResult != null)
                {
                    return interupResult;
                }

                var text = dialogContext.Context.Activity.Text.ToLowerInvariant();

                if (text.Contains("recent"))
                {
                    await SendRecentOrdersCardMessage(dialogContext.Context, cancellationToken);
                }
                else
                {
                    await SendEntreCardMessage(dialogContext.Context, cancellationToken);
                }

                return new DialogTurnResult(DialogTurnStatus.Complete);
            }

            return await base.OnBeginDialogAsync(dialogContext, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext dialogContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dialogContext.Context.Activity.Type == ActivityTypes.Message)
            {
                // Can add interups here
            }

            return null;
        }

        private async Task SendEntreCardMessage(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(
                MessageFactory.Attachment(new CardResource("EntreOptions.json").AsAttachment()), cancellationToken);
        }

        private async Task SendRecentOrdersCardMessage(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var latestOrders = await _cateringDb.GetRecentOrdersAsync();
            var users = latestOrders.Items;
            await turnContext.SendActivityAsync(
                MessageFactory.Attachment(new CardResource("RecentOrders.json").AsAttachment(
                    new
                    {
                        users = users.Select(u => new
                        {
                            lunch = new
                            {
                                entre = u.Lunch.Entre,
                                drink = u.Lunch.Drink,
                                orderTimestamp = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(u.Lunch.OrderTimestamp, "Pacific Standard Time").ToString("g")
                            }
                        }).ToList()
                    })), cancellationToken);
        }
    }
}
