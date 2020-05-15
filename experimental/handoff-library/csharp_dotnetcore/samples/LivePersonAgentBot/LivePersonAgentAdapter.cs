using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

