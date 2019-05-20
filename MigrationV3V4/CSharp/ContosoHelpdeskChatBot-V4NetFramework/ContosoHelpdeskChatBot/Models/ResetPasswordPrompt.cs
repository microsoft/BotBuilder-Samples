namespace ContosoHelpdeskChatBot.Models
{
    using Bot.Builder.Community.Dialogs.FormFlow;
    using System;

    [Serializable]
    public class ResetPasswordPrompt
    {
        [Prompt("Please provide four digit pass code")]
        public int PassCode { get; set; }
    }
}
