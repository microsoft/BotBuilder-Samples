// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Asp_Mvc_Bot.Controllers
{
    [Route("bot5")]
    public class Bot5Controller : Controller
    {
        private IIntegrationBot _bot;

        public Bot5Controller(IIntegrationBot bot)
        {
            _bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            // deserialize the incoming Activity - this step can be done by MVC
            var activity = HttpHelper.FromRequest(Request);

            // process the inbound activity with the bot
            var invokeResponse = await _bot.ProcessAsync(Request.Headers["Authorization"], activity);

            // write the response, potentially serializing the InvokeResponse - this step can be done by MVC
            HttpHelper.ToResponse(Response, invokeResponse);
        }
    }
}
