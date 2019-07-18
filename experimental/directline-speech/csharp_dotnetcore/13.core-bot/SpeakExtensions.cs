using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace CoreBot
{
    public static class SpeakExtensions
    {
        public static IActivity Speak(string message)
        {
            var activity = MessageFactory.Text(message);
            string body = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
              <voice name='Microsoft Server Speech Text to Speech Voice (en-US, JessaRUS)'>" +
              $"{message}" + "</voice></speak>";
            activity.Speak = body;
            return activity;
        }
    }
}
