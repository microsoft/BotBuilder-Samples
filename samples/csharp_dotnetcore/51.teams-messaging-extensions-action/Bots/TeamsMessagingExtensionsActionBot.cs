// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.BotBuilderSamples.Models;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsMessagingExtensionsActionBot : TeamsActivityHandler
    {
        ConcurrentDictionary<string, Survey> _surveys;

        /// <summary>
        /// Constructs a TeamsMessagingExtensionBot.
        /// </summary>
        /// <param name="surveys">Used to store Surveys and Responses.
        /// In a production environment, this would be your favorite storage provider.
        /// See Startup.cs, where this ConcurrentDictionary is created.
        /// </param>
        public TeamsMessagingExtensionsActionBot(ConcurrentDictionary<string, Survey> surveys)
        {
            _surveys = surveys ?? throw new ArgumentNullException(nameof(surveys));
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            switch (action.CommandId)
            {
                // These commandIds are defined in the Teams App Manifest.
                case "createSurvey":
                    return await CreateSurveyCommand(turnContext, action);

                case "shareMessage":
                    return ShareMessageCommand(turnContext, action);
                default:
                    throw new NotImplementedException($"Invalid CommandId: {action.CommandId}");
            }
        }

        private Task<MessagingExtensionActionResponse> CreateSurveyCommand(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action)
        {
            // Create Survey action command was chosen.  The parameters entered in the Create Survey Card
            // dialog will arrive in the MessagingExtensionAction.Data field.
            var createCardData = ((JObject)action.Data).ToObject<SurveyCardExampleData>();
            string surveyId = Guid.NewGuid().ToString();
            var card = AdaptiveCardHelper.CreateSurveyCard(createCardData, surveyId);
            
            var survey = Survey.NewSurvey(surveyId);
            survey.Question = createCardData.Question;
            survey.CreatedByUserId = turnContext.Activity.From.Id;
            survey.CreatedByUserName = turnContext.Activity.From.Name;

            _surveys.TryAdd(surveyId, survey);

            return Task.FromResult(new MessagingExtensionActionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    AttachmentLayout = "list",
                    Type = "result",
                    Attachments = new List<MessagingExtensionAttachment>()
                    {
                        new MessagingExtensionAttachment
                        {
                            Content = card,
                            ContentType = AdaptiveCard.ContentType,
                            Preview =  card.ToAttachment(),
                        },
                    },
                },
            });
        }

        private MessagingExtensionActionResponse ShareMessageCommand(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action)
        {
            // The user has chosen to share a message by choosing the 'Share Message' context menu command.
            var heroCard = new HeroCard
            {
                Title = $"{action.MessagePayload.From?.User?.DisplayName} orignally sent this message:",
                Text = action.MessagePayload.Body.Content,
            };

            if (action.MessagePayload.Attachments != null && action.MessagePayload.Attachments.Count > 0)
            {
                // This sample does not add the MessagePayload Attachments.  This is left as an
                // exercise for the user.
                heroCard.Subtitle = $"({action.MessagePayload.Attachments.Count} Attachments not included)";
            }
            
            // This Messaging Extension example allows the user to check a box to include an image with the
            // shared message.  This demonstrates sending custom parameters along with the message payload.
            var includeImage = ((JObject)action.Data)["includeImage"]?.ToString();
            if (!string.IsNullOrEmpty(includeImage) && bool.TrueString == includeImage)
            {
                heroCard.Images = new List<CardImage>
                {
                    new CardImage { Url = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU" },
                };
            }

            return new MessagingExtensionActionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "result",
                    AttachmentLayout = "list",
                    Attachments = new List<MessagingExtensionAttachment>()
                    {
                        new MessagingExtensionAttachment
                        {
                            Content = heroCard,
                            ContentType = HeroCard.ContentType,
                            Preview = heroCard.ToAttachment(),
                        },
                    },
                },
            };
        }

        protected override Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            // The user has requested to see the Results of a Survey by clicking the
            // View Results button.  (See AdaptiveCardHelper.CreateSurveyCard)
            var surveyId = (taskModuleRequest.Data as JObject)["SurveyId"].ToString();
            Survey survey = null;
            AdaptiveCard card = null;
            int cardHeight = 100;

            // Since this example is using a ConcurrentDictionary in memory for surveys, it is
            // possible the application has been restarted and a user has requested results for
            // a survey no longer in memory.  In this case, just add a new default survey.
            if (!_surveys.TryGetValue(surveyId, out survey))
            {
                survey = Survey.NewSurvey(surveyId);
                _surveys.TryAdd(surveyId, survey);

                card = AdaptiveCardHelper.CreateNoResultsCard();
            }
            else
            {
                card = AdaptiveCardHelper.CreateSurveyResultsCard(survey);
                var factSet = card.Body.FirstOrDefault(f => f.Type == AdaptiveFactSet.TypeName) as AdaptiveFactSet;

                // Calculate the height of the Task Module based on the number of replies.
                cardHeight = 150 + Math.Min(50 * factSet.Facts.Count, 600);
            }

            return Task.FromResult(new TaskModuleResponse
            {
                Task = new TaskModuleContinueResponse
                {
                    Value = new TaskModuleTaskInfo()
                    {
                        Card = card.ToAttachment(),
                        Height = cardHeight,
                        Width = 600,
                        Title = "Survey Results",
                    },
                },
            });
        }

        protected override Task OnTeamsMessagingExtensionCardButtonClickedAsync(ITurnContext<IInvokeActivity> turnContext, JObject cardData, CancellationToken cancellationToken)
        {
            // When a user submits a survey, their response choices will arrive in cardData.
            // Since the SurveyId was embedded in the button's AdaptiveSubmitAction.Data field,
            // it will also be present in cardData. (see AdaptiveCardHelper.CreateSurveyCard)
            var surveyId = cardData["SurveyId"].ToString();
            Survey survey = null;

            // Since this example is using a ConcurrentDictionary in memory for surveys, it is
            // possible the application has been restarted and a user has submitted a survey
            // no longer in memory.  In this case, just add a new survey for the response.
            if (!_surveys.TryGetValue(surveyId, out survey))
            {
                survey = Survey.NewSurvey(surveyId);
                _surveys.TryAdd(surveyId, survey);
            }

            survey.Responses.Add(new SurveyResponse
            {
                Answer = cardData["Answer"]?.ToString(),
                Choices = cardData["Choices"]?.ToString(),
                UserId = turnContext.Activity.From.Id,
                UserName = turnContext.Activity.From.Name,
            });

            return Task.CompletedTask;
        }

        protected override Task<TaskModuleResponse> OnTeamsTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            // This method is to handle the 'Close' button on the Results Task Module.
            return Task.FromResult<TaskModuleResponse>(null);
        }
    }
}
