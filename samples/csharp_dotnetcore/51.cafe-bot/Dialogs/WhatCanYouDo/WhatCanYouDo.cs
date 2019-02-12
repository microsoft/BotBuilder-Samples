// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public class WhatCanYouDo : Dialog
    {
        // This dialog's name. Also matches the name of the intent from ../Dispatcher/Resources/cafeDispatchModel.lu
        // LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
        public const string Name = "What_can_you_do";

        public WhatCanYouDo()
        : base(Name)
        {
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options, CancellationToken cancellationToken)
        {
            var activity = dc.Context.Activity.CreateReply();
            activity.Attachments = new List<Attachment> { Helpers.CreateAdaptiveCardAttachment(new[] { ".", "Dialogs", "WhatCanYouDo", "Resources", "whatCanYouDoCard.json" }) };
            await dc.Context.SendActivityAsync(activity);
            await dc.Context.SendActivityAsync("Pick a query from the card or you can use the suggestions below.");
            return await dc.EndDialogAsync();
        }
    }
}
