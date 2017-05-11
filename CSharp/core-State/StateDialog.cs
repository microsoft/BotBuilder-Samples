namespace StateBot
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class StateDialog : IDialog<object>
    {
        private const string HelpMessage = "\n * If you want to know which city I'm using for my searches type 'current city'. \n * Want to change the current city? Type 'change city to cityName'. \n * Want to change it just for your searches? Type 'change my city to cityName'";
        private bool userWelcomed;

        public async Task StartAsync(IDialogContext context)
        {
            string defaultCity;

            if (!context.ConversationData.TryGetValue(ContextConstants.CityKey, out defaultCity))
            {
                defaultCity = "Seattle";
                context.ConversationData.SetValue(ContextConstants.CityKey, defaultCity);
            }

            await context.PostAsync($"Welcome to the Search City bot. I'm currently configured to search for things in {defaultCity}");
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            string userName;

            if (!context.UserData.TryGetValue(ContextConstants.UserNameKey, out userName))
            {
                PromptDialog.Text(context, this.ResumeAfterPrompt, "Before get started, please tell me your name?");
                return;
            }

            if (!this.userWelcomed)
            {
                this.userWelcomed = true;
                await context.PostAsync($"Welcome back {userName}! Remember the rules: {HelpMessage}");

                context.Wait(this.MessageReceivedAsync);
                return;
            }

            if (message.Text.Equals("current city", StringComparison.InvariantCultureIgnoreCase))
            {
                string userCity;

                var city = context.ConversationData.GetValue<string>(ContextConstants.CityKey);

                if (context.PrivateConversationData.TryGetValue(ContextConstants.CityKey, out userCity))
                {
                    await context.PostAsync($"{userName}, you have overridden the city. Your searches are for things in  {userCity}. The default conversation city is {city}.");
                }
                else
                {
                    await context.PostAsync($"Hey {userName}, I'm currently configured to search for things in {city}.");
                }
            } 
            else if (message.Text.StartsWith("change city to", StringComparison.InvariantCultureIgnoreCase))
            {
                var newCity = message.Text.Substring("change city to".Length).Trim();
                context.ConversationData.SetValue(ContextConstants.CityKey, newCity);

                await context.PostAsync($"All set {userName}. From now on, all my searches will be for things in {newCity}.");
            }
            else if (message.Text.StartsWith("change my city to", StringComparison.InvariantCultureIgnoreCase))
            {
                var newCity = message.Text.Substring("change my city to".Length).Trim();
                context.PrivateConversationData.SetValue(ContextConstants.CityKey, newCity);

                await context.PostAsync($"All set {userName}. I have overridden the city to {newCity} just for you.");
            } 
            else
            {
                string city;

                if (!context.PrivateConversationData.TryGetValue(ContextConstants.CityKey, out city))
                {
                    city = context.ConversationData.GetValue<string>(ContextConstants.CityKey);
                }

                await context.PostAsync($"{userName}, wait a few seconds. Searching for '{message.Text}' in '{city}'...");
                await context.PostAsync($"https://www.bing.com/search?q={HttpUtility.UrlEncode(message.Text)}+in+{HttpUtility.UrlEncode(city)}");
            }

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var userName = await result;
                this.userWelcomed = true;

                await context.PostAsync($"Welcome {userName}! {HelpMessage}");

                context.UserData.SetValue(ContextConstants.UserNameKey, userName);
            }
            catch (TooManyAttemptsException)
            {
            }

            context.Wait(this.MessageReceivedAsync);
        }
    }
}
