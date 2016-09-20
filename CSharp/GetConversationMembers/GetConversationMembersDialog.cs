namespace GetConversationMembersBot
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class GetConversationMembersDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public async virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IActivity> result)
        {
            var activity = (Activity)await result;

            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded != null && activity.MembersAdded.Any())
                {
                    string membersAdded = string.Join(
                        ", ", 
                        activity.MembersAdded.Select(
                            newMember => (newMember.Id != activity.Recipient.Id) ? $"{newMember.Name} (Id: {newMember.Id})" 
                                            : $"{activity.Recipient.Name} (Id: {activity.Recipient.Id})"));

                    await context.PostAsync($"Welcome {membersAdded}");
                }

                if (activity.MembersRemoved != null && activity.MembersRemoved.Any())
                {
                    string membersRemoved = string.Join(
                        ", ", 
                        activity.MembersRemoved.Select(
                            removedMember => (removedMember.Id != activity.Recipient.Id) ? $"{removedMember.Name} (Id: {removedMember.Id})" : string.Empty));

                    await context.PostAsync($"The following members {membersRemoved} were removed or left the conversation :(");
                }
            }

            if (activity.Type == ActivityTypes.Message)
            {
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                {
                    var client = scope.Resolve<IConnectorClient>();
                    var activityMembers = await client.Conversations.GetConversationMembersAsync(activity.Conversation.Id);

                    string members = string.Join(
                        "\n ", 
                        activityMembers.Select(
                            member => (member.Id != activity.Recipient.Id) ? $"* {member.Name} (Id: {member.Id})"
                                        : $"* {activity.Recipient.Name} (Id: {activity.Recipient.Id})"));

                    await context.PostAsync($"These are the members of this conversation: \n {members}");
                }
            }

            context.Wait(this.MessageReceivedAsync);
        }
    }
}
