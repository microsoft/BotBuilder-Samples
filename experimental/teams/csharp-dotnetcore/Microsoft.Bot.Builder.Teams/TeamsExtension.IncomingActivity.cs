// <copyright file="TeamsExtension.IncomingActivity.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Teams extension methods which operate on incoming activity.
    /// </summary>
    public partial class TeamsContext
    {
        /// <summary>
        /// Gets the type of event.
        /// </summary>
        public string EventType
        {
            get
            {
                return this.GetTeamsChannelData().EventType;
            }
        }

        /// <summary>
        /// Gets info about the team in which this activity fired.
        /// </summary>
        public TeamInfo Team
        {
            get
            {
                return this.GetTeamsChannelData().Team;
            }
        }

        /// <summary>
        /// Gets info about the channel in which this activity fired.
        /// </summary>
        public ChannelInfo Channel
        {
            get
            {
                return this.GetTeamsChannelData().Channel;
            }
        }

        /// <summary>
        /// Gets tenant info for the activity.
        /// </summary>
        public TenantInfo Tenant
        {
            get
            {
                return this.GetTeamsChannelData().Tenant;
            }
        }

        /// <summary>
        /// Gets the general channel for a team.
        /// </summary>
        /// <returns>Channel data for general channel.</returns>
        /// <exception cref="ArgumentException">Failed to process channel data in Activity.</exception>
        /// <exception cref="ArgumentNullException">ChannelData missing in Activity.</exception>
        public ChannelInfo GetGeneralChannel()
        {
            if (this.turnContext.Activity.ChannelData != null)
            {
                TeamsChannelData channelData = this.turnContext.Activity.GetChannelData<TeamsChannelData>();

                if (channelData != null && channelData.Team != null)
                {
                    return new ChannelInfo
                    {
                        Id = channelData.Team.Id,
                    };
                }

                throw new ArgumentException("Failed to process channel data in Activity. ChannelData is missing Team property.");
            }
            else
            {
                throw new ArgumentException("ChannelData missing in Activity");
            }
        }

        /// <summary>
        /// Creates a reply for the General channel of the team.
        /// </summary>
        /// <param name="text">Reply text.</param>
        /// <param name="locale">Locale information.</param>
        /// <returns>New reply activity with General channel channel data.</returns>
        public Activity CreateReplyToGeneralChannel(string text = null, string locale = null)
        {
            TeamsChannelData channelData = this.turnContext.Activity.GetChannelData<TeamsChannelData>();
            Activity replyActivity = this.turnContext.Activity.CreateReply(text, locale);

            replyActivity.ChannelData = new TeamsChannelData
            {
                Channel = this.GetGeneralChannel(),
                Team = channelData.Team,
                Tenant = channelData.Tenant,
            }.AsJObject();

            return replyActivity;
        }

        /// <summary>
        /// Gets the Teams channel data associated with the current activity.
        /// </summary>
        /// <returns>Teams channel data <see cref="TeamsChannelData"/>Teams channel data for current activity.</returns>
        /// <exception cref="ArgumentNullException">ChannelData missing in Activity.</exception>
        public TeamsChannelData GetTeamsChannelData()
        {
            if (this.turnContext.Activity.ChannelData != null)
            {
                TeamsChannelData channelData = this.turnContext.Activity.GetChannelData<TeamsChannelData>();

                return channelData;
            }
            else
            {
                throw new ArgumentNullException("ChannelData missing in Activity");
            }
        }

        /// <summary>
        /// Gets the activity text without mentions.
        /// </summary>
        /// <returns>Text without mentions.</returns>
        public string GetActivityTextWithoutMentions()
        {
            Activity activity = this.turnContext.Activity;

            // Case 1. No entities.
            if (activity.Entities?.Count == 0)
            {
                return activity.Text;
            }

            IEnumerable<Entity> mentionEntities = activity.Entities.Where(entity => entity.Type.Equals("mention", StringComparison.OrdinalIgnoreCase));

            // Case 2. No Mention entities.
            if (!mentionEntities.Any())
            {
                return activity.Text;
            }

            // Case 3. Mention entities.
            string strippedText = activity.Text;

            mentionEntities.ToList()
                .ForEach(entity =>
                {
                    strippedText = strippedText.Replace(entity.GetAs<Mention>().Text, string.Empty);
                });

            return strippedText.Trim();
        }

        /// <summary>
        /// Gets the conversation parameters for create or get direct conversation.
        /// </summary>
        /// <param name="user">The user to create conversation with.</param>
        /// <returns>Conversation parameters to get or create direct conversation (1on1) between bot and user.</returns>
        public ConversationParameters GetConversationParametersForCreateOrGetDirectConversation(ChannelAccount user)
        {
            return new ConversationParameters()
            {
                Bot = this.turnContext.Activity.Recipient,
                ChannelData = JObject.FromObject(
                    new TeamsChannelData
                    {
                        Tenant = new TenantInfo
                        {
                            Id = this.Tenant.Id,
                        },
                    },
                    JsonSerializer.Create(new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    })),
                Members = new List<ChannelAccount>() { user },
            };
        }

        /// <summary>
        /// Checks if the request is a O365 connector card action query.
        /// </summary>
        /// <returns>True is activity is a actionable card query, false otherwise.</returns>
        public bool IsRequestO365ConnectorCardActionQuery()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.Equals("actionableMessage/executeAction", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets O365 connector card action query data.
        /// </summary>
        /// <returns>O365 connector card action query data.</returns>
        public O365ConnectorCardActionQuery GetO365ConnectorCardActionQueryData()
        {
            return this.turnContext.Activity.Value.AsJObject().ToObject<O365ConnectorCardActionQuery>();
        }

        /// <summary>
        /// Checks if the request is a signin state verification query.
        /// </summary>
        /// <returns>True is activity is a signin state verification query, false otherwise.</returns>
        public bool IsRequestSigninStateVerificationQuery()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.Equals("signin/verifyState", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets signin state verification query data.
        /// </summary>
        /// <returns>Signin state verification query data.</returns>
        public SigninStateVerificationQuery GetSigninStateVerificationQueryData()
        {
            return this.turnContext.Activity.Value.AsJObject().ToObject<SigninStateVerificationQuery>();
        }

        /// <summary>
        /// Determines whether activity is a file consent user's response.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if activity is a file consent user's response; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRequestFileConsentResponse()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.Equals("fileConsent/invoke", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the file consent user's response data.
        /// Check this by calling <see cref="IsRequestFileConsentResponse"/>.
        /// </summary>
        /// <returns>File consent response (accept or reject) chosen by user on file consent card.</returns>
        public FileConsentCardResponse GetFileConsentQueryData()
        {
            return this.turnContext.Activity.Value.AsJObject().ToObject<FileConsentCardResponse>();
        }

        /// <summary>
        /// Checks if the activity is a messaging extension query.
        /// </summary>
        /// <returns>True is activity is a messaging extension query, false otherwise.</returns>
        public bool IsRequestMessagingExtensionQuery()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.Equals("composeExtension/query", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the messaging extension query data.
        /// </summary>
        /// <returns>Messaging extension query data.</returns>
        public MessagingExtensionQuery GetMessagingExtensionQueryData()
        {
            return this.turnContext.Activity.Value.AsJObject().ToObject<MessagingExtensionQuery>();
        }

        /// <summary>
        /// Determines whether request is an app-based link query.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if request is an app-based link query; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRequestAppBasedLinkQuery()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.Equals("composeExtension/queryLink", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the app-based link query data.
        /// Check this by calling <see cref="IsRequestAppBasedLinkQuery"/>.
        /// </summary>
        /// <returns>App-based link query data.</returns>
        public AppBasedLinkQuery GetAppBasedLinkQueryData()
        {
            return this.turnContext.Activity.Value.AsJObject().ToObject<AppBasedLinkQuery>();
        }

        /// <summary>
        /// Determines whether request is a messaging extension action for fetch task.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if request is a messaging extension action for fetch task; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRequestMessagingExtensionFetchTask()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.Equals("composeExtension/fetchTask", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether request is a messaging extension action for submit action.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if request is a messaging extension action for submit action; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRequestMessagingExtensionSubmitAction()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.Equals("composeExtension/submitAction", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the compose extension action data.
        /// Check this by calling <see cref="IsRequestMessagingExtensionFetchTask"/> or <see cref="IsRequestMessagingExtensionSubmitAction"/>.
        /// </summary>
        /// <returns>Compose extension action data.</returns>
        public MessagingExtensionAction GetMessagingExtensionActionData()
        {
            return this.turnContext.Activity.Value.AsJObject().ToObject<MessagingExtensionAction>();
        }

        /// <summary>
        /// Determines whether request is for task module fetch.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if request is for task module fetch; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRequestTaskModuleFetch()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.Equals("task/fetch", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether request is for task module submit.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if request is for task module submit; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRequestTaskModuleSubmit()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.Equals("task/submit", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the task module request query data.
        /// Check this by calling <see cref="IsRequestTaskModuleFetch"/> or <see cref="IsRequestTaskModuleSubmit"/>.
        /// </summary>
        /// <returns>Task module request query data.</returns>
        public TaskModuleRequest GetTaskModuleRequestData()
        {
            return this.turnContext.Activity.Value.AsJObject().ToObject<TaskModuleRequest>();
        }
    }
}
