// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Bot.Builder.Community.Dialogs.FormFlow;

namespace ContosoHelpdeskChatBot.Models
{
    public class LocalAdminPrompt
    {
        [Prompt("What is the machine name to add you to local admin group?")]
        public string MachineName { get; set; }

        [Prompt("How many days do you need the admin access?")]
        public int? AdminDuration { get; set; }
    }
}
