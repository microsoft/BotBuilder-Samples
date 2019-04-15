// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Providers.PersonalityChat
{
    /// <summary>
    /// Personality chat reply.
    /// </summary>
    public class ChatReply
    {
        /// <summary>
        /// Gets or sets answer from personality chat.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets score from personality chat.
        /// </summary>
        public double Score { get; set; }
    }
}
