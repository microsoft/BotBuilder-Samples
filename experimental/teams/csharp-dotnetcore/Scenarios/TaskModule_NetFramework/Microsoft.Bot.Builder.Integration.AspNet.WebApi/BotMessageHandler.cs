// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Integration.AspNet.WebApi.Handlers
{
    public sealed class BotMessageHandler : BotMessageHandlerBase
    {
        public static readonly string RouteName = "BotFramework - Message Handler";

        public BotMessageHandler(IAdapterIntegration adapter)
            : base(adapter)
        {
        }

        protected override async Task<InvokeResponse> ProcessMessageRequestAsync(HttpRequestMessage request, IAdapterIntegration adapter, BotCallbackHandler botCallbackHandler, CancellationToken cancellationToken)
        {
            var activity = await request.Content.ReadAsAsync<Activity>(BotMessageHandlerBase.BotMessageMediaTypeFormatters, cancellationToken).ConfigureAwait(false);

#pragma warning disable UseConfigureAwait // Use ConfigureAwait
            var invokeResponse = await adapter.ProcessActivityAsync(
                request.Headers.Authorization?.ToString(),
                activity,
                botCallbackHandler,
                cancellationToken);
#pragma warning restore UseConfigureAwait // Use ConfigureAwait

            return invokeResponse;
        }
    }
}
