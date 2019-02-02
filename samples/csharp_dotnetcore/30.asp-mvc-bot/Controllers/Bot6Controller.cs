// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;

namespace Asp_Mvc_Bot.Controllers
{
    [Route("bot6")]
    public class Bot6Controller : Controller
    {
        private IBotFrameworkAdapter _adapter;
        private IBot _bot;

        public Bot6Controller(IBotFrameworkAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            await _adapter.ProcessAsync(Request, Response, _bot);
        }
    }
}
