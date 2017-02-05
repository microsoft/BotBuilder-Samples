using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace Zummer.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly ILifetimeScope scope;
        
        public MessagesController(ILifetimeScope scope)
        {
            SetField.NotNull(out this.scope, nameof(scope), scope);
        }

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity, CancellationToken token)
        {
            if (activity != null)
            {
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        using (var beginLifetimeScope = DialogModule.BeginLifetimeScope(this.scope, activity))
                        {
                            var postToBot = beginLifetimeScope.Resolve<IPostToBot>();
                            await postToBot.PostAsync(activity, token);
                        }

                        break;
                    default:
                        await this.HandleSystemMessage(activity);
                        break;
                }
            }

            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        private async Task<Activity> HandleSystemMessage(Activity activity)
        {
            if (activity.Type != null)
            {
                if (activity.Type == ActivityTypes.DeleteUserData)
                {
                }
                else if (activity.Type == ActivityTypes.ConversationUpdate)
                {
                }
                else if (activity.Type == ActivityTypes.ContactRelationUpdate)
                {
                }
                else if (activity.Type == ActivityTypes.Typing)
                {
                }
                else if (activity.Type == ActivityTypes.Ping)
                {
                }
            }

            return null;
        }
    }
}