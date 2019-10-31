// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using AdaptiveCards;
using Microsoft.Bot.Schema.Teams;
using Microsoft.BotBuilderSamples.Models;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    public static class AdaptiveCardHelper
    {
        public static SurveyCardExampleData CreateSurveyExampleData(AdaptiveCard adaptiveCard)
        {
            string userText = (adaptiveCard.Body[1] as AdaptiveTextBlock).Text;
            var choiceSet = adaptiveCard.Body[3] as AdaptiveChoiceSetInput;

            return new SurveyCardExampleData
            {
                Question = userText,
                IsMultiSelect = choiceSet.IsMultiSelect ? "true" : "false",
                Option1 = choiceSet.Choices[0].Title,
                Option2 = choiceSet.Choices[1].Title,
                Option3 = choiceSet.Choices[2].Title,
            };
        }

        public static AdaptiveCard CreateNoResultsCard()
        {
            return new AdaptiveCard
            {
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock("No results.") { Weight = AdaptiveTextWeight.Bolder },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Type = AdaptiveSubmitAction.TypeName,
                        Title = "Close",
                    },
                },
            };
        }

        public static AdaptiveCard CreateSurveyResultsCard(Survey survey)
        {
            var bodyElements = new List<AdaptiveElement>()
            {
                new AdaptiveColumnSet
                {
                    Columns = new List<AdaptiveColumn>()
                    {
                        new AdaptiveColumn
                        {
                            Items = new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock { Text = "Question:", Weight = AdaptiveTextWeight.Bolder },
                                new AdaptiveTextBlock { Text = "Created By:", Weight = AdaptiveTextWeight.Bolder },
                            }
                        },
                        new AdaptiveColumn
                        {
                            Items = new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock { Text = survey.Question },
                                new AdaptiveTextBlock { Text = survey.CreatedByUserName },
                            }
                        }
                    }
                },
            };

            bodyElements.Add(new AdaptiveTextBlock { Separation = AdaptiveSeparationStyle.Strong, Separator = true });
            
            var factSet = new AdaptiveFactSet { Facts = new List<AdaptiveFact>() };
            bodyElements.Add(factSet);

            var userNamesColumn = new AdaptiveColumn { Items = new List<AdaptiveElement>(), Style = AdaptiveContainerStyle.Emphasis, Width = AdaptiveColumnWidth.Auto };
            var responsesColumn = new AdaptiveColumn { Items = new List<AdaptiveElement>(), Width = AdaptiveColumnWidth.Stretch };

            foreach (var response in survey.Responses)
            {
                factSet.Facts.Add(new AdaptiveFact { Title = response.UserName.Substring(0, 20), Value = response.Choices + "\n\n" + response.Answer });
            }
            
            return new AdaptiveCard
            {
                Body = bodyElements,
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Type = AdaptiveSubmitAction.TypeName,
                        Title = "Close",
                    },
                },
            };
        }

        public static AdaptiveCard CreateSurveyCard(SurveyCardExampleData data, string surveyId)
        {
            return new AdaptiveCard
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock("Survey") { Weight = AdaptiveTextWeight.Bolder },
                    new AdaptiveTextBlock($"{data.Question}") { Id = "Question", Wrap = true },
                    new AdaptiveTextInput() { Id = "Answer", Placeholder = "Answer here..." },
                    new AdaptiveChoiceSetInput
                    {
                        Type = AdaptiveChoiceSetInput.TypeName,
                        Id = "Choices",
                        IsMultiSelect = bool.Parse(data.IsMultiSelect),
                        Choices = new List<AdaptiveChoice>
                        {
                            new AdaptiveChoice() { Title = data.Option1, Value = data.Option1 },
                            new AdaptiveChoice() { Title = data.Option2, Value = data.Option2 },
                            new AdaptiveChoice() { Title = data.Option3, Value = data.Option3 },
                        },
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Type = AdaptiveSubmitAction.TypeName,
                        Title = "Submit Answer",
                        Data = new JObject { { "SurveyId", surveyId } },
                    },
                    new TaskModuleAction("View Results", @"{ ""SurveyId"": """ + surveyId + @""" }").ToAdaptiveCardAction()
                },
            };
        }
    }
}
