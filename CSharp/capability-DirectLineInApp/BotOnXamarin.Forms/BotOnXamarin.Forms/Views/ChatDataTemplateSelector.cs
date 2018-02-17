using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using BotOnXamarin.Forms.Views.DataTemplates;
using BotOnXamarin.Forms.Models;

namespace BotOnXamarin.Forms.Views
{
    public class ChatDataTemplateSelector : DataTemplateSelector
    {

        DataTemplate MessageFromUserTemplate;
        DataTemplate MessageFromBotTemplate;

        public ChatDataTemplateSelector()
        {
            MessageFromUserTemplate = new DataTemplate(typeof(SentByUser));
            MessageFromBotTemplate = new DataTemplate(typeof(SentByBot));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var msg = item as BotMessage;
            DataTemplate template = null;

            if (msg == null)
                return null;

            if(msg.ISent == true)
            {
                template = MessageFromUserTemplate;
            }
            else
            {
                template = MessageFromBotTemplate;
            }

            return template;
        }
    }
}
