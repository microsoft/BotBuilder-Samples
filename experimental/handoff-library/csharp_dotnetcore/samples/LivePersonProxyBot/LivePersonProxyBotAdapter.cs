// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using LivePersonConnector;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LivePersonProxyBot
{
    public class LivePersonProxyBotAdapter : LivePersonAdapter
    {
        public LivePersonProxyBotAdapter(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger, HandoffMiddleware handoffMiddleware, LoggingMiddleware loggingMiddleware)
            : base(configuration, logger, handoffMiddleware)
        {
            Use(loggingMiddleware);
        }
    }
}
