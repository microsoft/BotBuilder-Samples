using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using TeamsTasksBot.Helper;
using TeamsTasksBot.Models;
using System;
using Newtonsoft.Json;
using Microsoft.Bot.Connector;
using Microsoft.BotBuilderSamples;

namespace TeamsTasksBot.Bots
{
    public class TasksBot : TeamsActivityHandler
    {

        public TasksBot()
        {
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {

            await base.OnTurnAsync(turnContext, cancellationToken);
        }

        protected override async Task<InvokeResponse> OnTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            var activityValue = turnContext.Activity.Value.ToString();

            Models.BotFrameworkCardValue<string> action;
            try
            {
                action = JsonConvert.DeserializeObject<Models.TaskModuleActionData<string>>(activityValue).Data;
            }
            catch (Exception)
            {
                action = JsonConvert.DeserializeObject<Models.BotFrameworkCardValue<string>>(activityValue);
            }

            Models.TaskInfo taskInfo = GetTaskInfo(action.Data);
            Models.TaskEnvelope taskEnvelope = new Models.TaskEnvelope
            {
                Task = new Models.TeamsTask()
                {
                    Type = Models.TaskType.Continue,
                    TaskInfo = taskInfo
                }
            };
            System.Diagnostics.Debug.WriteLine(taskEnvelope.Task.TaskInfo.ToJson());

            var response = new InvokeResponse()
            {
                Status = (int)System.Net.HttpStatusCode.OK,
                Body = taskEnvelope
            };

            return response;
        }

        protected override async Task<InvokeResponse> OnTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            Activity reply = (turnContext.Activity as Activity).CreateReply("Received = " + turnContext.Activity.Value.ToString());
            var connector = turnContext.TurnState.Get<IConnectorClient>();
            connector.Conversations.ReplyToActivity(reply);

            return new InvokeResponse() { Status = (int)System.Net.HttpStatusCode.OK };
        }

        protected override Task OnUnrecognizedActivityTypeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return base.OnUnrecognizedActivityTypeAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = (turnContext.Activity as Activity).CreateReply();

            ThumbnailCard card = GetTaskModuleOptions();
            Attachment adaptiveCard = GetTaskModuleOptionsAdaptiveCard();

            reply.Attachments.Add(card.ToAttachment());
            reply.Attachments.Add(adaptiveCard);

            await turnContext.SendActivityAsync(reply);
        }
        private static Attachment GetTaskModuleOptionsAdaptiveCard()
        {
            var card = new AdaptiveCard()
            {
                Body = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock(){Text="Task Module Invocation from Adaptive Card",Weight=AdaptiveTextWeight.Bolder,Size=AdaptiveTextSize.Large}
                    },
                Actions = new List<AdaptiveAction>()
                {
                     new AdaptiveSubmitAction()
                    {
                        Title = TaskModuleUIConstants.YouTube.ButtonTitle,
                        Data = new AdaptiveCardValue<string>() { Data = TaskModuleUIConstants.YouTube.Id}
                    },
                      new AdaptiveSubmitAction()
                    {
                        Title = TaskModuleUIConstants.PowerApp.ButtonTitle,
                        Data = new AdaptiveCardValue<string>() { Data = TaskModuleUIConstants.PowerApp.Id}
                    },
                    new AdaptiveSubmitAction()
                    {
                        Title = TaskModuleUIConstants.CustomForm.ButtonTitle,
                        Data = new AdaptiveCardValue<string>() { Data = TaskModuleUIConstants.CustomForm.Id}
                    },
                    new AdaptiveSubmitAction()
                    {
                        Title = TaskModuleUIConstants.AdaptiveCard.ButtonTitle,
                        Data = new AdaptiveCardValue<string>() { Data = TaskModuleUIConstants.AdaptiveCard.Id }
                    },
                    new AdaptiveOpenUrlAction()
                    {
                        Title = "Task Module - Deeplink",
                        Url = new Uri(DeeplinkHelper.DeepLink),
                        Id = "TaskModuleDeeplink"
                    }
               },
            };
            return new Attachment() { ContentType = AdaptiveCard.ContentType, Content = card };
        }

        private static ThumbnailCard GetTaskModuleOptions()
        {
            ThumbnailCard card = new ThumbnailCard();
            card.Title = "Task Module Invocation from Thumbnail Card";
            card.Buttons = new List<CardAction>();
            card.Buttons.Add(new CardAction()
            {
                Type = "invoke",
                Title = TaskModuleUIConstants.YouTube.ButtonTitle,
                Value = new BotFrameworkCardValue<string>()
                {
                    Data = TaskModuleUIConstants.YouTube.Id
                }
            });

            card.Buttons.Add(new CardAction()
            {
                Type = "invoke",
                Title = TaskModuleUIConstants.PowerApp.ButtonTitle,
                Value = new BotFrameworkCardValue<string>()
                {
                    Data = TaskModuleUIConstants.PowerApp.Id
                }
            });
            card.Buttons.Add(new CardAction()
            {
                Type = "invoke",
                Title = TaskModuleUIConstants.CustomForm.ButtonTitle,
                Value = new BotFrameworkCardValue<string>()
                {
                    Data = TaskModuleUIConstants.CustomForm.Id
                }
            });
            card.Buttons.Add(new CardAction()
            {
                Type = "invoke",
                Title = TaskModuleUIConstants.AdaptiveCard.ButtonTitle,
                Value = new BotFrameworkCardValue<string>()
                {
                    Data = TaskModuleUIConstants.AdaptiveCard.Id
                }
            });
            
            card.Buttons.Add(new CardAction(type: "openUrl",title: "Task Module - Deeplink",image: null, value: DeeplinkHelper.DeepLink, text: "Deeplink"));
            return card;
        }


        private static Models.TaskInfo GetTaskInfo(string actionInfo)
        {
            Models.TaskInfo taskInfo = new Models.TaskInfo();
            switch (actionInfo)
            {
                case TaskModuleIds.YouTube:
                    taskInfo.Url = taskInfo.FallbackUrl = ApplicationSettings.BaseUrl + "/" + TaskModuleIds.YouTube;
                    SetTaskInfo(taskInfo, TaskModuleUIConstants.YouTube);
                    break;
                case TaskModuleIds.PowerApp:
                    taskInfo.Url = taskInfo.FallbackUrl = ApplicationSettings.BaseUrl + "/" + TaskModuleIds.PowerApp;
                    SetTaskInfo(taskInfo, TaskModuleUIConstants.PowerApp);
                    break;
                case TaskModuleIds.CustomForm:
                    taskInfo.Url = taskInfo.FallbackUrl = ApplicationSettings.BaseUrl + "/" + TaskModuleIds.CustomForm;
                    SetTaskInfo(taskInfo, TaskModuleUIConstants.CustomForm);
                    break;
                case TaskModuleIds.AdaptiveCard:
                    taskInfo.Card = AdaptiveCardHelper.GetAdaptiveCard();
                    SetTaskInfo(taskInfo, TaskModuleUIConstants.AdaptiveCard);
                    break;
                default:
                    break;
            }
            return taskInfo;
        }

        private static void SetTaskInfo(Models.TaskInfo taskInfo, UIConstants uIConstants)
        {
            taskInfo.Height = uIConstants.Height;
            taskInfo.Width = uIConstants.Width;
            taskInfo.Title = uIConstants.Title.ToString();
        }

    }
}
