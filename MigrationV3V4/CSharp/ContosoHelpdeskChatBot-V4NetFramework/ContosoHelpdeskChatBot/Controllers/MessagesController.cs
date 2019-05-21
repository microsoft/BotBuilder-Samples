// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ContosoHelpdeskChatBot
{
    public class MessagesController : ApiController
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;
        
        public MessagesController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        public async Task<HttpResponseMessage> Post()
        {
            var response = new HttpResponseMessage();

            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await _adapter.ProcessAsync(Request, response, _bot);
            return response;
        }
    }
}
