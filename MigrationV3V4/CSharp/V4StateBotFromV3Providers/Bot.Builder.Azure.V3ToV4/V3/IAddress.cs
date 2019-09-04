// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
namespace Bot.Builder.Azure.V3V4
{
    /// <summary>
    /// This interface existed in Bot Builder SDK V3, but was not brought forward to SDK V4.
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    public interface IAddress
    {
        string BotId { get; }
        string ChannelId { get; }
        string UserId { get; }
        string ConversationId { get; }
        string ServiceUrl { get; }
    }
}
