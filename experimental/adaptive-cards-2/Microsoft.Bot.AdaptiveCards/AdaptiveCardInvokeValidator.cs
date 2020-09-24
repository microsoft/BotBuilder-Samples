using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Bot.AdaptiveCards
{
    public class AdaptiveCardInvokeValidator
    {
        public static bool IsAdaptiveCardAction(ITurnContext turnContext)
        {
            return turnContext.Activity.Type == ActivityTypes.Invoke &&
                string.Equals(AdaptiveCardAction.Name, turnContext.Activity.Name);
        }

        public static AdaptiveCardInvoke ValidateRequest(ITurnContext turnContext)
        {
            var activity = turnContext.Activity;
            AdaptiveCardInvoke request = null;

            if (activity.Value == null)
            {
                AdaptiveCardActionException.BadRequest("Missing value property");
            }

            try
            {
                request = ((JObject)turnContext.Activity.Value).ToObject<AdaptiveCardInvoke>();
            }
            catch(Exception)
            {
                AdaptiveCardActionException.BadRequest("Value property is not properly formed");
            }

            if (request.Action == null)
            {
                AdaptiveCardActionException.BadRequest("Missing action property");
            }

            if (request.Action.Type != AdaptiveCardsConstants.ActionExecute)
            {
                AdaptiveCardActionException.ActionNotSupported(request.Action.Type);
            }

            return request;
        }

        public static T ValidateAction<T>(AdaptiveCardInvoke request)
        {
            if (request.Action.Data == null)
            {
                AdaptiveCardActionException.BadRequest("Missing data property");
            }

            try
            {
                return ((JObject)request.Action.Data).ToObject<T>();
            }
            catch(Exception)
            {
                AdaptiveCardActionException.BadRequest("Action.Data property is not properly formed");
            }

            return default(T);
        }
    }
}
