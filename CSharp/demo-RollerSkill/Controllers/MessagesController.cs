namespace RollerSkillBot
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using RollerSkillBot.Dialogs;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            await Conversation.SendAsync(activity, () => new RootDispatchDialog());

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }
    }
}