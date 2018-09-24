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
        public WhatCanYouDo()
            : base(nameof(WhatCanYouDo))
        {
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options, CancellationToken cancellationToken)
        {
            await dc.Context.SendActivityAsync(new Activity()
            {
                Attachments = new List<Attachment> { Helpers.CreateAdaptiveCardAttachment(@"..\..\WhatCanYouDo\Resources\whatCanYouDoCard.json") },
            }).ConfigureAwait(false);
            await dc.Context.SendActivityAsync("Pick a query from the card or you can use the suggestions below.");
            return await dc.EndDialogAsync();
        }
    }
}
