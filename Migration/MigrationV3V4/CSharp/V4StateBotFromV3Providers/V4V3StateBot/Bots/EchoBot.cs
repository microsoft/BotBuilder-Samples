using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bot.Builder.Azure.V3V4;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace V4StateBot.Bots
{
    public class EchoBot<T> : ActivityHandler where T : Dialog
    {
        private readonly V3V4State _userState;
        private readonly ConversationState _conversationState;
        private readonly Dialog Dialog;

        public EchoBot(V3V4State userState, ConversationState conversationState, T dialog)
        {
            _userState = userState;
            _conversationState = conversationState;
            Dialog = dialog;
        }

        public async override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Run the Dialog with the new message Activity.
            await Dialog.Run(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        //protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    var v3State = await _v3State.GetAsync(turnContext, () => new BotData("", new Dictionary<string, object>()));
        //    var greetingState = v3State.GetProperty<GreetingState>("V3TestGreeting");

        //    if (greetingState != null && !string.IsNullOrEmpty(greetingState.Name))
        //    {
        //        object changingNameObj = false;
        //        bool changingName = v3State.GetProperty<bool>("ChangingName");

        //        if (changingName)
        //        {
        //            greetingState.Name = turnContext.Activity.Text;
        //            v3State.SetProperty<bool>("ChangingName", false);
        //            v3State.SetProperty<GreetingState>("V3TestGreeting", greetingState);
        //        }
        //        else if(turnContext.Activity.Text.Equals("change name", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            v3State.SetProperty<bool>("ChangingName", true);

        //            await turnContext.SendActivityAsync("v4: Okay, what would you like to change your name to?");
        //            await _userState.SaveChangesAsync(turnContext);
        //            return;
        //        }

        //        await turnContext.SendActivityAsync(MessageFactory.Text($"v4: Hi {greetingState.Name} from {greetingState.City}. You said: {turnContext.Activity.Text}"), cancellationToken);
        //    }
        //    else
        //    {
        //        v3State.SetProperty<GreetingState>("V3TestGreeting",  new GreetingState() { Name = "Eric auto", City = "Seattle (auto)" });
        //        v3State.SetProperty<TestDataClass>("V3TestDataClass", new TestDataClass()
        //        {
        //            TestIntField = 11,
        //            TestStringField = "auto string field",
        //            TestTuple = new Tuple<string, string>("item 1 auto", "item 2 auto"),
        //        });
        //        v3State.SetProperty<bool>("AskedName", true);
        //        v3State.SetProperty<bool>("Test", true);

        //        await turnContext.SendActivityAsync(MessageFactory.Text($"v4: (greetingState was NOT present...auto saved) Echo: {turnContext.Activity.Text}"), cancellationToken);
        //    }

        //    await _userState.SaveChangesAsync(turnContext);
        //}

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and Welcome!"), cancellationToken);
                }
            }
        }
    }
}