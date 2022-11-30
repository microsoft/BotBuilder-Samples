// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using AdaptiveCards;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    public static class AdaptiveCardHelper
    {
        public static ExampleData CreateExampleData(AdaptiveCard adaptiveCard)
        {
            string userText = (adaptiveCard.Body[1] as AdaptiveTextBlock).Text;
            var choiceSet = adaptiveCard.Body[3] as AdaptiveChoiceSetInput;
            var attributionFlag = (adaptiveCard.Body[4] as AdaptiveTextBlock).Text.Split(":")[1];

            return new ExampleData
            {
                Question = userText,
                MultiSelect = choiceSet.IsMultiSelect ? "true" : "false",
                UserAttributionSelect=attributionFlag,
                Option1 = choiceSet.Choices[0].Title,
                Option2 = choiceSet.Choices[1].Title,
                Option3 = choiceSet.Choices[2].Title,
            };
        }

        public static AdaptiveCard CreateAdaptiveCardEditor(ExampleData exampleData = null)
        {
            var cardData = exampleData ?? new ExampleData();

            return new AdaptiveCard
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock("This is an Adaptive Card within a Task Module")
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                    },
                    new AdaptiveTextBlock("Enter text for Question:"),
                    new AdaptiveTextInput() { Id = "Question", Placeholder = "Question text here", Value = cardData.Question },
                    new AdaptiveTextBlock("Options for Question:"),
                    new AdaptiveTextBlock("Is Multi-Select:"),
                    new AdaptiveChoiceSetInput
                    {
                        Type = AdaptiveChoiceSetInput.TypeName,
                        Id = "MultiSelect",
                        Value = cardData.MultiSelect,
                        IsMultiSelect = false,
                        Choices = new List<AdaptiveChoice>
                        {
                            new AdaptiveChoice() { Title = "True", Value = "true" },
                            new AdaptiveChoice() { Title = "False", Value = "false" },
                        },
                    },
                    new AdaptiveTextInput() { Id = "Option1", Placeholder = "Option 1 here", Value = cardData.Option1 },
                    new AdaptiveTextInput() { Id = "Option2", Placeholder = "Option 2 here", Value = cardData.Option2 },
                    new AdaptiveTextInput() { Id = "Option3", Placeholder = "Option 3 here", Value = cardData.Option3 },

                    new AdaptiveTextBlock("Do you want to send this card on behalf of User?"),
                    new AdaptiveChoiceSetInput
                    {
                        Type = AdaptiveChoiceSetInput.TypeName,
                        Id = "UserAttributionSelect",
                        Value = cardData.UserAttributionSelect,
                        Choices = new List<AdaptiveChoice>
                        {
                            new AdaptiveChoice() { Title = "True", Value = "true" },
                            new AdaptiveChoice() { Title = "False", Value = "false" },
                        },
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Type = AdaptiveSubmitAction.TypeName,
                        Title = "Submit",
                        Data = new JObject { { "submitLocation", "messagingExtensionFetchTask" } },
                    },
                },
            };
        }

        public static AdaptiveCard CreateAdaptiveCard(ExampleData data)
        {
            return new AdaptiveCard
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock("Adaptive Card from Task Module") { Weight = AdaptiveTextWeight.Bolder },
                    new AdaptiveTextBlock($"{data.Question}") { Id = "Question" },
                    new AdaptiveTextInput() { Id = "Answer", Placeholder = "Answer here..." },
                    new AdaptiveChoiceSetInput
                    {
                        Type = AdaptiveChoiceSetInput.TypeName,
                        Id = "Choices",
                        IsMultiSelect = bool.Parse(data.MultiSelect),
                        Choices = new List<AdaptiveChoice>
                        {
                            new AdaptiveChoice() { Title = data.Option1, Value = data.Option1 },
                            new AdaptiveChoice() { Title = data.Option2, Value = data.Option2 },
                            new AdaptiveChoice() { Title = data.Option3, Value = data.Option3 },
                        },
                    },
                  new AdaptiveTextBlock("Sending card on behalf of user is set to:"+$"{data.UserAttributionSelect}") {
                      Id = "AttributionChoice"
                  },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Type = AdaptiveSubmitAction.TypeName,
                        Title = "Submit",
                        Data = new JObject { { "submitLocation", "messagingExtensionSubmit" } },
                    },
                },
            };
        }
    }
}
