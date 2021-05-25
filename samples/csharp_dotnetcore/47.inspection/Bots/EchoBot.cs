// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;

        public EchoBot(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationStateProp = _conversationState.CreateProperty<CustomState>("customState");
            var convProp = await conversationStateProp.GetAsync(turnContext, () => new CustomState { Value = 0 }, cancellationToken);

            var userStateProp = _userState.CreateProperty<CustomState>("customState");
            var userProp = await userStateProp.GetAsync(turnContext, () => new CustomState { Value = 0 }, cancellationToken);

            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text} conversation state: {convProp.Value} user state: {userProp.Value}"), cancellationToken);

            convProp.Value++;
            userProp.Value++;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }
    }
}
