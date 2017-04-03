namespace LuisActions.Samples.Bot
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Connector;
    using Microsoft.Cognitive.LUIS.ActionBinding.Bot;
    using Samples;
    using Samples.Models;

    [Serializable]
    public class RootDialog : LuisActionDialog<object>
    {
        public RootDialog() : base(
            new Assembly[] { typeof(FindHotelsAction).Assembly },
            (action, context) =>
            {
                // Here you can implement a callback to hydrate action contexts as per request

                // For example:
                // If your action is related with a 'Booking' intent, then you could do something like:
                // BookingSystem.Hydrate(action) - hydrate action context already stored within some repository
                // (ex. using a booking ref that you can get from the context somehow)

                // To simply showcase the idea, here we are setting the checkin/checkout dates for 1 night
                // when the user starts a contextual intent related with the 'FindHotelsAction'

                // So if you simply write 'Change location to Madrid' the main action will have required parameters already set up
                // and, as in this case the context is an IDialogContext, you can get the user information for any purpose
                if (action is FindHotelsAction)
                {
                    (action as FindHotelsAction).Checkin = DateTime.Today;
                    (action as FindHotelsAction).Checkout = DateTime.Today.AddDays(1);
                }
            },
            new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LUIS_ModelId"], ConfigurationManager.AppSettings["LUIS_SubscriptionKey"])))
        {
        }

        [LuisIntent("FindHotels")]
        [LuisIntent("TimeInPlace")]
        public async Task IntentActionResultHandlerAsync(IDialogContext context, object actionResult)
        {
            // we know these actions return a string for their related intents,
            // although you could have individual handlers for each intent
            var message = context.MakeMessage();

            message.Text = actionResult != null ? actionResult.ToString() : "Cannot resolve your query";

            await context.PostAsync(message);
        }

        [LuisIntent("WeatherInPlace")]
        public async Task WeatherInPlaceActionHandlerAsync(IDialogContext context, object actionResult)
        {
            // we know the action for this intent returns a WeatherInfo, so we cast the result
            var weatherInfo = (WeatherInfo)actionResult;

            var message = context.MakeMessage();
            message.Text = weatherInfo != null
                ? $"The current weather in {weatherInfo.Location}, {weatherInfo.Country} is {weatherInfo.Condition} (humidity {weatherInfo.Humidity}%)"
                : "Weather information not available";

            await context.PostAsync(message);
        }

        [LuisIntent("FindAirportByCode")]
        public async Task FindAirportByCodeActionHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> message, object actionResult)
        {
            var messageText = (await message).Text;

            // we know the action for this intent returns an AirportInfo, so we cast the result
            var airportInfo = (AirportInfo)actionResult;

            var reply = context.MakeMessage();
            reply.Text = airportInfo != null
                ? $"{airportInfo.Code} corresponds to \"{airportInfo.Name}\" which is located in {airportInfo.City}, {airportInfo.Country} [{airportInfo.Location}]"
                : $"We could not find the airport for your \"{messageText}\" request, please try a different one!";

            await context.PostAsync(reply);
        }
    }
}