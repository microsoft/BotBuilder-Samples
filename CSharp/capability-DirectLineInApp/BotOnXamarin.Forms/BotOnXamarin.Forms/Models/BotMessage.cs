using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotOnXamarin.Forms.Models
{
    public class BotMessage
    {
        public string ActivityId { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
        public bool ISent { get; set; }

        public BotMessage(string activityId, string msg, DateTime time)
        {
            ActivityId = activityId;
            Time = time;
            Content = msg;
        }
        public BotMessage()
        {

        }
    }
}
