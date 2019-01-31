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

        public async Task ProcessAsync(HttpRequest request, HttpResponse response, BotCallbackHandler callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            // deserialize the incoming Activity
            var activity = HttpHelper.FromRequest(request);

            // create the adapter we will be using - we should be able to DI in the adapter
            var adapter = new BotFrameworkAdapter(new SimpleCredentialProvider());

            // grab the auth header from the inbound http request
            var authHeader = request.Headers["Authorization"];

            // process the inbound activity with the bot
            var invokeResponse = await ProcessActivityAsync(authHeader, activity, callback, cancellationToken);

            // write the response, potentially serializing the InvokeResponse
            HttpHelper.ToResponse(response, invokeResponse);
        }
    }
}
