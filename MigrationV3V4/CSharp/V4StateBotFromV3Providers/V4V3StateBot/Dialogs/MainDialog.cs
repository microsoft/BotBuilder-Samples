using Bot.Builder.Azure.V3V4;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using V4StateBot.Models;

namespace V4StateBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private const string V3GreetingPropertyName = "V3TestGreeting";
        private const string ChangingNamePropertyName = "ChangingName";
        private const string V3StatePropertyName = "V3State";

        private V3V4State _userState;
        private readonly IStatePropertyAccessor<BotData> _v3State;

        public MainDialog(V3V4State userState)
            : base(nameof(MainDialog))
        {
            _userState = userState;
            _v3State = userState.CreateProperty<BotData>("V3State");

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                NameStep,
                CityStep,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var v3State = await _v3State.GetAsync(stepContext.Context, () => new BotData("", new Dictionary<string, object>()));
            var greetingState = v3State.GetProperty<GreetingState>(V3GreetingPropertyName);

            if (greetingState != null && !string.IsNullOrEmpty(greetingState.Name))
            {
                if (stepContext.Context.Activity.Text.Equals("change name", StringComparison.InvariantCultureIgnoreCase))
                {
                    v3State.SetProperty<bool>(ChangingNamePropertyName, true);

                    await stepContext.Context.SendActivityAsync("v4: Okay, what would you like to change your name to?");
                    await _userState.SaveChangesAsync(stepContext.Context);
                    return new DialogTurnResult(DialogTurnStatus.Waiting);
                }

                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"v4: Hi {greetingState.Name} from {greetingState.City}. You said: {stepContext.Context.Activity.Text}"), cancellationToken);
                return new DialogTurnResult(DialogTurnStatus.Complete);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("v4: Hi, what is your name?") }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> NameStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var v3State = await _v3State.GetAsync(stepContext.Context, () => new BotData("", new Dictionary<string, object>()));
            var greetingState = v3State.GetProperty<GreetingState>(V3GreetingPropertyName) ?? new GreetingState();

            greetingState.Name = stepContext.Context.Activity.Text;
            v3State.SetProperty<GreetingState>(V3GreetingPropertyName, greetingState);

            bool changingName = v3State.GetProperty<bool>(ChangingNamePropertyName);

            if (changingName)
            {
                v3State.SetProperty<bool>(ChangingNamePropertyName, false);
                await stepContext.Context.SendActivityAsync($"v4: Okay, your name is now stored as {greetingState.Name}");
                return new DialogTurnResult(DialogTurnStatus.Complete);
            }

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text($"v4: Ok, I've got your name as {greetingState.Name}.  What city do you live in?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> CityStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var v3State = await _v3State.GetAsync(stepContext.Context, () => new BotData("", new Dictionary<string, object>()));
            var greetingState = v3State.GetProperty<GreetingState>(V3GreetingPropertyName);
            greetingState.City = stepContext.Context.Activity.Text;
            v3State.SetProperty<GreetingState>(V3GreetingPropertyName, greetingState);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"v4: Hi {greetingState.Name} from {greetingState.City}."), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
