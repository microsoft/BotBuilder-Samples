// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LivePersonAgentBot
{
    public class LivePersonAgentAdapter : BotFrameworkHttpAdapter
    {
        public LivePersonAgentAdapter(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger, HandoffMiddleware loggingMiddleware)
            : base(configuration, logger)
        {
            Use(loggingMiddleware);
        }
    }
}

