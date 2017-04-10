using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace simpleSendMessage.Controllers
{
    //This controller exists just to prove that we can trigger that proactive conversation even from compeltely outside the bot's code
    //Let's say you have a web service or some backend system that needs to trigger it, this is how you would do that
    public class CustomWebAPIController : ApiController
    {
        [HttpGet]

        [Route("api/CustomWebAPI")]
        public async Task<HttpResponseMessage> SendMessage()
        {
            try
            {
                if (!string.IsNullOrEmpty(ConversationStarter.fromId))
                {
                    await ConversationStarter.Resume(ConversationStarter.conversationId, ConversationStarter.channelId); //We don't need to wait for this, just want to start the interruption here

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent($"<html><body>Message sent, thanks.</body></html>", System.Text.Encoding.UTF8, @"text/html");
                    return resp;
                }
                else
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent($"<html><body>You need to talk to the bot first so it can capture your details.</body></html>", System.Text.Encoding.UTF8, @"text/html");
                    return resp;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}