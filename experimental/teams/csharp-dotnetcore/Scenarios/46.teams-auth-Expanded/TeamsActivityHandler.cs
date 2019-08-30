// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public class TeamsActivityHandler : ActivityHandler
    {
        public override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity == null)
            {
                throw new ArgumentException($"{nameof(turnContext)} must have non-null Activity.");
            }

            if (turnContext.Activity.Type == null)
            {
                throw new ArgumentException($"{nameof(turnContext)}.Activity must have non-null Type.");
            }

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Invoke:
                    return OnInvokeActivityAsync(new DelegatingTurnContext<IInvokeActivity>(turnContext), cancellationToken);

                default:
                    return base.OnTurnAsync(turnContext, cancellationToken);
            }
        }

        protected virtual Task OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            switch (turnContext.Activity.Name)
            {
                case "signin/verifyState":
                    return OnSigninVerifyStateAsync(turnContext, cancellationToken);

                default:
                    return Task.CompletedTask;
            }
        }

        protected virtual Task OnSigninVerifyStateAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
