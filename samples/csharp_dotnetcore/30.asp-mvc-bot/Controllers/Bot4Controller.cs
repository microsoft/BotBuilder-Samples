// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;

namespace Asp_Mvc_Bot.Controllers
{
    [Route("bot4")]
    public class Bot4Controller : Controller
    {
        public Bot4Controller()
        {
        }

        [HttpPost]
        public async Task PostAsync()
        {
            // deserialize the incoming Activity - this step can be done by MVC
            var activity = HttpHelper.FromRequest(Request);

            // create the adapter we will be using - we should be able to DI in the adapter
            var adapter = new BotFrameworkAdapter(new SimpleCredentialProvider());

            // create the bot against this adapter - we should be able to DI in the bot
            var bot = new MyBot(adapter);

            // process the inbound activity with the bot
            var invokeResponse = await bot.ProcessAsync(Request.Headers["Authorization"], activity);

            // write the response, potentially serializing the InvokeResponse - this step can be done by MVC
            HttpHelper.ToResponse(Response, invokeResponse);
        }
    }
}
