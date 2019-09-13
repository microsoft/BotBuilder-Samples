// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Microsoft.BotBuilderSamples;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace TaskModule.Bots
{
    public class TaskModuleBot : TeamsActivityHandler
    {
        const string AdaptiveCardTaskModuleId = "adaptivecard";
        static TaskModuleProperties AdaptiveCard => new TaskModuleProperties(700, 500, "Adaptive Card: Inputs", AdaptiveCardTaskModuleId, "Adaptive Card");

        string _baseUrl;
        string _webRootPath;

        public TaskModuleBot(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _baseUrl = configuration["BaseUrl"];
            _webRootPath = hostingEnvironment.ContentRootPath;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Attachment(GetTaskModuleOptions().ToAttachment());
            await turnContext.SendActivityAsync(reply);
        }

        protected override async Task OnTeamsTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("Bot Received = " + turnContext.Activity.Value.ToString());
            await turnContext.SendActivityAsync(reply);
        }

        protected override Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return base.OnTeamsMessagingExtensionFetchTaskAsync(turnContext, cancellationToken);
        }

        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            var activityValue = turnContext.Activity.Value.ToString();

            var action = JsonConvert.DeserializeObject<TaskModuleActionData<string>>(activityValue).Data;
            
            var taskInfo = new TaskModuleTaskInfo();
            taskInfo.Card = AdaptiveCardHelper.GetAdaptiveCard(_webRootPath);
            taskInfo.Height = AdaptiveCard.Height;
            taskInfo.Width = AdaptiveCard.Width;
            taskInfo.Title = AdaptiveCard.Title.ToString();

            return new TaskModuleResponse
            {
                Task = new TaskModuleContinueResponse()
                {
                    Type = "continue",
                    Value = taskInfo
                }
            };
        }

        private static HeroCard GetTaskModuleOptions()
        {
            var card = new HeroCard();
            card.Title = "Task Module Invocation from Hero Card";
            card.Buttons = new List<CardAction>();
            card.Buttons.Add(new CardAction()
            {
                Type = "invoke",
                Title = AdaptiveCard.ButtonTitle,
                Value = new BotFrameworkCardValue<string>()
                {
                    Data = AdaptiveCard.Id
                }
            });

            return card;
        }

        private class TaskModuleProperties
        {
            public TaskModuleProperties(int width, int height, string title, string id, string buttonTitle)
            {
                Width = width;
                Height = height;
                Title = title;
                Id = id;
                ButtonTitle = buttonTitle;
            }

            public int Height { get; set; }
            public int Width { get; set; }
            public string Title { get; set; }
            public string ButtonTitle { get; set; }
            public string Id { get; set; }
        }

        private class BotFrameworkCardValue<T>
        {
            [JsonProperty("type")]
            public object Type { get; set; } = "task/fetch";

            [JsonProperty("data")]
            public T Data { get; set; }
        }

        private class TaskModuleActionData<T>
        {
            [JsonProperty("data")]
            public BotFrameworkCardValue<T> Data { get; set; }
        }
    }
}
