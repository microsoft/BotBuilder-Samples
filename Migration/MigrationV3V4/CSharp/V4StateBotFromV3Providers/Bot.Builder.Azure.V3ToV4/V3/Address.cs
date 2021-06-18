// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using Microsoft.Bot.Schema;

namespace Bot.Builder.Azure.V3V4
{
    /// <summary>
    /// This class existed in Bot Builder SDK V3, but was not brought forward to SDK V4.
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    internal class Address : IAddress
    {
        public Address() { }

        public Address(IActivity activity)
        {
            this.BotId = activity.Recipient.Id;
            this.ChannelId = activity.ChannelId;
            this.UserId = activity.From.Id;
            this.ConversationId = activity.Conversation.Id;
            this.ServiceUrl = activity.ServiceUrl;
        }

        public string BotId { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty;
        public string ServiceUrl { get; set; } = string.Empty;
    }    
}
