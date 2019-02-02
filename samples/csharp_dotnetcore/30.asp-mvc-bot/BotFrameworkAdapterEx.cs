using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;

namespace Asp_Mvc_Bot
{
    public class BotFrameworkAdapterEx : BotFrameworkAdapter, IBotFrameworkAdapter
    {
        public BotFrameworkAdapterEx(ICredentialProvider credentialProvider)
            : base(credentialProvider)
        {
        }

        public async Task ProcessAsync(HttpRequest request, HttpResponse response, IBot bot, CancellationToken cancellationToken = default(CancellationToken))
        {
            // deserialize the incoming Activity
            var activity = HttpHelper.FromRequest(request);

            // grab the auth header from the inbound http request
            var authHeader = request.Headers["Authorization"];

            // process the inbound activity with the bot
            var invokeResponse = await ProcessActivityAsync(authHeader, activity, bot.OnTurnAsync, cancellationToken);

            // write the response, potentially serializing the InvokeResponse
            HttpHelper.ToResponse(response, invokeResponse);
        }
    }
}
