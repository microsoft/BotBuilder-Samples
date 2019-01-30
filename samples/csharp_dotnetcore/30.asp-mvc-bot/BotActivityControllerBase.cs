// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// It is common practice to switch on the type of inbound Activity in the bot logic. This class implements that
    /// type switch for the common Message and ConversationUpdate Activities.
    /// </summary>
    public class BotActivityControllerBase : BotControllerBase
    {
        /// <summary>
        /// This is an implementation of OnTurnAsync that provides the typical switch on Activity type.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                return OnMessageActivityAsync(turnContext, cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                return OnConversationUpdateActivityAsync(turnContext, cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.Event)
            {
                return OnEventActivityAsync(turnContext, cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.DeleteUserData)
            {
                return OnDeleteUserDataActivityAsync(turnContext, cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                return OnContactRelationUpdateActivityAsync(turnContext, cancellationToken);
            }
            else
            {
                return OnOtherActivityAsync(turnContext, cancellationToken);
            }
        }

        /// <summary>
        /// Override this method to add custom processing for Message Activities.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected virtual Task OnMessageActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method to add custom processing for ConversationUpdate Activities. The base implementation
        /// supports the common pattern of specific processing per newly added member.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected virtual async Task OnConversationUpdateActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.MembersAdded != null)
            {
                // an alternative design might just pass the collection on down rather than iterate here
                foreach (var member in turnContext.Activity.MembersAdded)
                {
                    // this can depend on the channel but some channels will create two conversation updates
                    // when teh conversation starts - one for the user and one for the bot
                    if (member.Id != turnContext.Activity.Recipient.Id)
                    {
                        await OnMemberAddedAsync(member, turnContext, cancellationToken);
                    }
                }

                return;
            }

            if (turnContext.Activity.MembersRemoved != null)
            {
                // an alternative design might just pass the collection on down rather than iterate here
                foreach (var member in turnContext.Activity.MembersRemoved)
                {
                    // TODO: verify whether this check is meaningful for remove
                    if (member.Id != turnContext.Activity.Recipient.Id)
                    {
                        await OnMemberRemovedAsync(member, turnContext, cancellationToken);
                    }
                }

                return;
            }
        }

        /// <summary>
        /// Override this method to add custom processing for each added member. For example a Welcome message.
        /// </summary>
        /// <param name="member">A <see cref="ChannelAccount"/> corresponding to the added member.</param>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected virtual Task OnMemberAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method to add custom processing for each member removed. For example a good-bye message.
        /// </summary>
        /// <param name="member">A <see cref="ChannelAccount"/> corresponding to the added member.</param>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected virtual Task OnMemberRemovedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method to add custom processing for Event Activity not covered in this sample.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected virtual Task OnEventActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method to add custom processing for DeleteUserData Activity not covered in this sample.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected virtual Task OnDeleteUserDataActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method to add custom processing for ContactRelationUpdate Activity not covered in this sample.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected virtual Task OnContactRelationUpdateActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            // TODO: some documentation as to when this Activity can be sent would be helpful
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method to add custom processing for any other type of Activity not covered in this sample.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected virtual Task OnOtherActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
