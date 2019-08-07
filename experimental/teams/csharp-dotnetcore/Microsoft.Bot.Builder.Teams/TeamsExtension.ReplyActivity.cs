// <copyright file="TeamsExtension.ReplyActivity.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Internal
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Teams extensions methods which operate on reply activity.
    /// </summary>
    public partial class TeamsContext
    {
        /// <summary>
        /// Adds the mention text to an existing activity.
        /// </summary>
        /// <typeparam name="T">Message activity type.</typeparam>
        /// <param name="activity">The activity.</param>
        /// <param name="mentionedEntity">The mentioned entity.</param>
        /// <param name="mentionText">The mention text. This is how you want to mention the entity.</param>
        /// <returns>Activity with added mention.</returns>
        /// <exception cref="Rest.ValidationException">Either mentioned user name or mentionText must have a value.</exception>
        public T AddMentionToText<T>(
            T activity,
            ChannelAccount mentionedEntity,
            string mentionText = null)
            where T : IMessageActivity
        {
            if (mentionedEntity == null || string.IsNullOrEmpty(mentionedEntity.Id))
            {
                throw new ArgumentNullException(nameof(mentionedEntity), "Mentioned entity and entity ID cannot be null");
            }

            if (string.IsNullOrEmpty(mentionedEntity.Name) && string.IsNullOrEmpty(mentionText))
            {
                throw new ArgumentException("Either mentioned name or mentionText must have a value");
            }

            if (!string.IsNullOrWhiteSpace(mentionText))
            {
                mentionedEntity.Name = mentionText;
            }

            string mentionEntityText = string.Format("<at>{0}</at>", mentionedEntity.Name);
            activity.Text = activity.Text + " " + mentionEntityText;

            if (activity.Entities == null)
            {
                activity.Entities = new List<Entity>();
            }

            activity.Entities.Add(new Mention
            {
                Text = mentionEntityText,
                Mentioned = mentionedEntity,
            });

            return activity;
        }

        /// <summary>
        /// Notifies the user in direct conversation.
        /// </summary>
        /// <typeparam name="T">Type of message activity.</typeparam>
        /// <param name="replyActivity">The reply activity.</param>
        /// <returns>Modified activity.</returns>
        public T NotifyUser<T>(T replyActivity)
            where T : IMessageActivity
        {
            TeamsChannelData channelData = replyActivity.GetChannelData<TeamsChannelData>() == null ?
                new TeamsChannelData() :
                replyActivity.GetChannelData<TeamsChannelData>();
            channelData.Notification = new NotificationInfo
            {
                Alert = true,
            };

            replyActivity.ChannelData = channelData.AsJObject();

            return replyActivity;
        }
    }
}
