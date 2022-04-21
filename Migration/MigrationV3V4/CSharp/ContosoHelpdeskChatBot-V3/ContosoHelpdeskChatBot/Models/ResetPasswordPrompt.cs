using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoHelpdeskChatBot.Models
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class ResetPasswordPrompt
    {
        [Prompt("Please provide four digit pass code")]
        public int PassCode { get; set; }
    }
}