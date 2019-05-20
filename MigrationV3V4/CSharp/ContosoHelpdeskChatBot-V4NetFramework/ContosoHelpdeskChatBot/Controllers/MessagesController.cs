using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Bot.Builder.Community.Dialogs.FormFlow;
using ContosoHelpdeskChatBot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;

namespace ContosoHelpdeskChatBot
{
    public class MessagesController : ApiController
    {
        IBotFrameworkHttpAdapter _adapter;
        IBot _bot;
        
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
