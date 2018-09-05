// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using WelcomeUser.State;

namespace WelcomeUser
{
    /// <summary>
    /// Represents a bot that can process incoming activities.
    /// For each interaction from the user, an instance of this class is called.
    /// For each Activity received, a new instance of this class is created.
    /// Objects that are expensive to construct, or have a lifetime.
    /// beyond the single Turn, should be carefully managed and chached - see Startup.cs.
    /// </summary>
    public class WelcomeUserBot : IBot
    {
        // generic message sent to user
        private const string _genericMessage = "This is a simple Welcome Bot sample.You can say \'intro\' to see the introduction card. If you are running this bot in the Bot Framework emulator, press the \'Start Over\' button to simulate user joining a bot or a channel";

        // the bot state accessor object. Use this to access specific state properties
        private readonly WelcomeUserStateAccessors _welcomeUserStateAccessors;

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeUserBot"/> class.
        /// Initalize WelcomeUserBot.
        /// </summary>
        /// <param name="statePropertyAccessor"> Bot state accessor object.</param>
        public WelcomeUserBot(WelcomeUserStateAccessors statePropertyAccessor)
        {
            _welcomeUserStateAccessors = statePropertyAccessor ?? throw new System.ArgumentNullException("state accessor can't be null");
        }

        /// <summary>
        /// Every Conversation turn for our WelcomeUser Bot will call this method, including
        /// any type of activiaties such as ConversationUpdate or ContactRelationUpdate which
        /// are sent when a user joins a conversation.
        /// This bot doesn't use any dialogs; it's "single turn" processing, meaning a single
        /// request and response, with no stateful conversation between user input.
        /// This bot does use UserState to keep track of first message users send to the bot.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = new CancellationToken())
        {
            // use state accessor to extract the didBotWelcomeUser flag
            var didBotWelcomeUser = await _welcomeUserStateAccessors.DidBotWelcomedUser.GetAsync(turnContext, () => false);

            if (turnContext.Activity.Type == ActivityTypes.Message && turnContext.Responded == false)
            {
                // Your bot should proactively send a welcome message to a personal chat the first time
                // (and only the first time) a user initiates a personal chat with your bot.
                if (didBotWelcomeUser == false)
                {
                    // update user state flag to reflect bot handled first user interaction.
                    await _welcomeUserStateAccessors.DidBotWelcomedUser.SetAsync(turnContext, true);

                    await turnContext.SendActivityAsync($"you first message ever to this bot ", cancellationToken: cancellationToken);
                }
                else
                {
                    var text = turnContext.Activity.Text.ToLowerInvariant();
                    switch (text)
                    {
                        case "hello":
                        case "hi":
                            await turnContext.SendActivityAsync($"You said {text}.", cancellationToken: cancellationToken);
                            await turnContext.SendActivityAsync(_genericMessage, cancellationToken: cancellationToken);
                            break;
                        case "intro":
                        case "help":
                            await SendIntroCardAsync(turnContext, cancellationToken);
                            break;
                        default:
                            await turnContext.SendActivityAsync(_genericMessage, cancellationToken: cancellationToken);
                            break;
                    }
                }
            }

            // greet when users are added to the conversation
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded.Any())
                {
                    // go over all new memebers, greet anyone that was not the target (recipient) of this message
                    // the 'bot' is the recipient for events from the channel
                    foreach (var member in turnContext.Activity.MembersAdded)
                    {
                        // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
                        // bot was added to the conversation
                        if (member.Id != turnContext.Activity.Recipient.Id)
                        {
                            await turnContext.SendActivityAsync($"Hi there - {member.Name}. Welcome to the \'Welcome User\' Bot. This bot will introduce you to Welcoming and greeting users.", cancellationToken: cancellationToken);
                            await turnContext.SendActivityAsync($"You are seeing this message, because the bot recieved atleast one \'ConversationUpdate\' event, indicating you (and possibly others) joined a conversation. If you are using the emulator, pressing the \'Start Over\' trigger thise event. \'ConversationUpdate\' event depends on the channel your bot is running. You can read more information https://aka.ms/about-botframewor-welcome-user", cancellationToken: cancellationToken);
                        }
                    }
                }
            }

            // default behaivor for all other type of events.
            else
            {
                var ev = turnContext.Activity.AsEventActivity();
                await turnContext.SendActivityAsync($"Received event: {ev.Name}");
            }
        }

        /// <summary>
        /// sends an adaptive card introduction.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        private static async Task SendIntroCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var response = turnContext.Activity.CreateReply("This is an hello world intro to Adaptive Cards");

            var introCard = File.ReadAllText(@".\Resources\IntroCard.json");

            response.Attachments = new List<Attachment>
            {
                new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(introCard),
                },
            };

            await turnContext.SendActivityAsync(response, cancellationToken);
        }
    }
}
