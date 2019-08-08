// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Handoff;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bot.Builder.EchoBot
{
    /// <summary>
    /// ActivityHandlerWithHandoff will be merged into ActivityHandler in SDK 4.6
    /// </summary>
    public class ActivityHandlerWithHandoff : ActivityHandler
    {
        public override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type == ActivityTypes.Handoff)
            {
                return OnHandoffActivityAsync(new DelegatingTurnContext<IHandoffActivity>(turnContext), cancellationToken);
            }

            return base.OnTurnAsync(turnContext, cancellationToken);
        }

        protected virtual Task OnHandoffActivityAsync(ITurnContext<IHandoffActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
