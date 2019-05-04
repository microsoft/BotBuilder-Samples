// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Providers.PersonalityChat
{
    using System.Collections.Generic;

    /// <summary>
    /// Personality chat response.
    /// </summary>
    public class PersonalityChatResponse
    {
        public List<ChatReply> Responses { get; set; }
    }
}
