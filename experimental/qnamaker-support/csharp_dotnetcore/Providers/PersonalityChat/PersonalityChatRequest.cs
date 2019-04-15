// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Providers.PersonalityChat
{
    /// <summary>
    /// Personality chat request.
    /// </summary>
    public class PersonalityChatRequest
    {
        public string PersonaName { get; set; }

        public string Query { get; set; }

        public int ResponseCount { get; set; }

        public string Metadata { get; set; }
    }
}
