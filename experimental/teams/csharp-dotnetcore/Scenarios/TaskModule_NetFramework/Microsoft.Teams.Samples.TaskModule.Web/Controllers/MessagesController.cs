// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;
using Microsoft.Teams.Samples.HelloWorld.Web.Bots;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.Teams.Samples.TaskModule.Web.Controllers
{
    public class MessagesController : ApiController
    {
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IBot Bot;

        public MessagesController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            Bot = bot;
            Adapter = adapter;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            var response = new HttpResponseMessage();
            await Adapter.ProcessAsync(Request, response, Bot);
            
            return response;
        }
    }
}
