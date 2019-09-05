using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Handlers;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamsTasksBot.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IBot Bot;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            Adapter = adapter;
            Bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            //Request.EnableBuffering();

            //using (var buffer = new MemoryStream())
            //{
            //    await Request.Body.CopyToAsync(buffer);
            //    buffer.Position = 0L;
            //    using (var bodyReader = new JsonTextReader(new StreamReader(buffer, Encoding.UTF8)))
            //    {
            //        Debug.WriteLine(BotMessageHandlerBase.BotMessageSerializer.Deserialize(bodyReader).ToString());
            //        buffer.Position = 0L;
            //    }
            //}

            //Request.Body.Position = 0;

            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await Adapter.ProcessAsync(Request, Response, Bot);
        }
    }
}
