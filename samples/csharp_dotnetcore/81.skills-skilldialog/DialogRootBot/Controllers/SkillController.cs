// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace Microsoft.BotBuilderSamples.DialogRootBot.Controllers
{
    /// <summary>
    /// A controller that handles skill replies to the bot.
    /// </summary>
    [ApiController]
    [Route("api/skills")]
    public class SkillController : ChannelServiceController
    {
        public SkillController(ChannelServiceHandlerBase handler)
            : base(handler)
        {
        }
    }
}
