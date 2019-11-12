// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters.Webex;

namespace WebexAdapterBot.Controllers
{
    [Route("api/webex")]
    [ApiController]
    public class WebexController : ControllerBase
    {
        private readonly WebexAdapter _adapter;
        private readonly IBot _bot;

        public WebexController(WebexAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost]
        [HttpGet]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await _adapter.ProcessAsync(Request, Response, _bot, default(CancellationToken));
        }
    }
}
