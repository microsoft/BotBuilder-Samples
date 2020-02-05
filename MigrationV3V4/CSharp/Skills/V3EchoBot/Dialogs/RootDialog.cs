using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.EchoBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            if (activity.Text.Contains("end") || activity.Text.Contains("stop"))
            {
                // Send End of conversation at the end.
                await context.PostAsync($"Ending conversation from the skill...");
                var endOfConversation = activity.CreateReply();
                endOfConversation.Type = ActivityTypes.EndOfConversation;
                endOfConversation.Code = EndOfConversationCodes.CompletedSuccessfully;
                await context.PostAsync(endOfConversation);
            }
            else
            {
                await context.PostAsync($"Echo (dotnet V3): {activity.Text}");
                await context.PostAsync($"Say 'end' or 'stop' and I'll end the conversation and back to the parent.");
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}
