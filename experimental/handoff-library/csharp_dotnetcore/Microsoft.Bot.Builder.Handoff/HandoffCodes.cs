// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. 

namespace Microsoft.Bot.Schema
{

    /// <summary>
    /// Defines values for codes for handoff events.
    /// </summary>
    public static class HandoffCodes
    {
        public const string Accepted = "accepted";
        public const string Failed = "failed";
        public const string TimedOut = "timedOut";
        public const string EndOfConversation = "endOfConversation";
        public const string TransferBack = "transferBack";
    }
}
