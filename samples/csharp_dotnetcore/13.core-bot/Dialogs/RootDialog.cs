// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        private Templates _templates;

        public RootDialog(IConfiguration configuration)
            : base(nameof(RootDialog))
        {
            Configuration = configuration;
            _templates = Templates.ParseFile(Path.Combine(".", "Dialogs", "RootDialog.lg"));

            // Create instance of adaptive dialog. 
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // There are no steps associated with this dialog.
                // This dialog will react to user input using its own Recognizer's output and Rules.

                // Add a recognizer to this adaptive dialog.
                // For this dialog, we will use the LUIS recognizer based on the FlightBooking.json
                // found under CognitiveModels folder.
                Recognizer = CreateRecognizer(configuration),

                // Add rules to respond to different events of interest
                //Rules = CreateRules()
                Generator = new TemplateEngineLanguageGenerator(_templates),
                Triggers = new List<OnCondition>()
                {
                    // Add a rule to welcome user
                    new OnConversationUpdateActivity() {Actions = WelcomeUserSteps()},
                    // Add additional rules to respond to other intents returned by the LUIS application.
                    // The intents here are based on intents defined in MainAdaptiveDialog.LU file
                    new OnIntent()
                    {
                        Intent = "Cancel",
                        Condition = "#Cancel.Score >= 0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Sure, cancelling that..."),
                            new CancelAllDialogs(),
                            new EndDialog()
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "Help",
                        Condition = "#Help.Score >= 0.8",
                        Actions = new List<Dialog>() {new SendActivity("${DisplayHelp()}")}
                    },
                    new OnIntent()
                    {
                        Intent = "Greeting",
                        Actions = new List<Dialog>() {new SendActivity("${BotOverview()}")}
                    },
                    new OnUnknownIntent()
                    {
                        Actions = new List<Dialog>() {new SendActivity("${UnknownIntent()}")}
                    },
                    new OnIntent("Book_flight")
                    {
                        Actions = BookFlightSteps()
                    },
                    new OnEndOfActions()
                    {
                        Actions = BookFlightSteps()
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private static List<Dialog> WelcomeUserSteps()
        {
            return new List<Dialog>()
            {
                // Iterate through membersAdded list and greet user added to the conversation.
                new Foreach()
                {
                    ItemsProperty = "turn.activity.membersAdded",
                    Actions = new List<Dialog>()
                    {
                        // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                        // Filter cases where the bot itself is the recipient of the message. 
                        new IfCondition()
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>()
                            {
                                new SendActivity("${WelcomeCard()}")
                            }
                        }
                    }
                },
            };
        }

        private static Recognizer CreateRecognizer(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["LuisAppId"]) || string.IsNullOrEmpty(configuration["LuisAPIKey"]) ||
                string.IsNullOrEmpty(configuration["LuisAPIHostName"]))
            {
                return new RegexRecognizer
                {
                    Intents = new List<IntentPattern>()
                    {
                        new IntentPattern() {Intent = "Help", Pattern = "help"},
                        new IntentPattern() {Intent = "Cancel", Pattern = "cancel"},
                        new IntentPattern() {Intent = "Greeting", Pattern = "greeting|hi|hello"},
                        new IntentPattern() {Intent = "Book_flight", Pattern = "book|flight"}
                    }
                };
            }

            return new LuisAdaptiveRecognizer()
            {
                ApplicationId = configuration["LuisAppId"],
                EndpointKey = configuration["LuisAPIKey"],
                Endpoint = configuration["LuisAPIHostName"]
            };
        }

        private List<Dialog> BookFlightSteps()
        {
            return new List<Dialog>()
            {
                // Save any entities returned by LUIS.
                new SetProperties()
                {
                    Assignments = new List<PropertyAssignment>()
                    {
                        new PropertyAssignment() {Property = "conversation.flightBooking", Value = "={}"},

                        // We will only save any geography city entities that explicitly have been classified as either fromCity or toCity.
                        new PropertyAssignment()
                        {
                            Property = "conversation.flightBooking.departureCity",
                            // Value is an expresson. @entityName is shorthand to refer to the value of an entity recognized.
                            // @xxx is same as turn.recognized.entities.xxx
                            Value = "=@fromCity.location"
                        },
                        new PropertyAssignment()
                        {
                            Property = "conversation.flightBooking.destinationCity", Value = "=@toCity.location"
                        },
                        new PropertyAssignment()
                        {
                            Property = "conversation.flightBooking.departureDate", Value = "=@datetime.timex[0]"
                        }
                    }
                },
                // Steps to book flight
                // Help and Cancel intents are always available since TextInput will always initiate
                // Consultation up the parent dialog chain to see if anyone else wants to take the user input.
                new TextInput()
                {
                    Property = "conversation.flightBooking.departureCity",
                    // Prompt property supports full language generation resolution.
                    // See here to learn more about language generation
                    // https://aka.ms/language-generation
                    Prompt = new ActivityTemplate("${PromptForMissingInformation()}"),
                    // We will allow interruptions as long as the user did not explicitly answer the question
                    // This property supports an expression so you can examine presence of an intent via #intentName, 
                    //    detect presence of an entity via @entityName etc. Interruption is allowed if the expression
                    //    evaluates to `true`. This property defaults to `true`.
                    AllowInterruptions = "!@fromCity || !@geographyV2",
                    // Value is an expression. Take first non null value. 
                    Value = "=coalesce(@fromCity.location, @geographyV2.location)"
                },
                // delete entity so it is not overconsumed as destination as well
                new DeleteProperty() {Property = "turn.recognized.entities.geographyV2"},
                new TextInput()
                {
                    Property = "conversation.flightBooking.destinationCity",
                    Prompt = new ActivityTemplate("${PromptForMissingInformation()}"),
                    AllowInterruptions = "!@toCity || !@geographyV2",
                    // Value is an expression. Take any recognized city name as fromCity
                    Value = "=coalesce(@toCity.location, @geographyV2.location)"
                },
                new DateTimeInput()
                {
                    Property = "conversation.flightBooking.departureDate",
                    Prompt = new ActivityTemplate("${PromptForMissingInformation()}"),
                    AllowInterruptions = "!@datetime",
                    Validations = new List<BoolExpression>()
                    {
                        // Prebuilt function that returns boolean true if we have a full, valid date.
                        "isDefinite(this.value[0].timex)"
                    },
                    InvalidPrompt = new ActivityTemplate("${Date.Invalid()}"),
                    // Value is an expression. Take any date time entity recognition as deparature date.
                    Value = "=@datetime.timex[0]"
                },
                new ConfirmInput()
                {
                    Property = "turn.bookingConfirmation",
                    Prompt = new ActivityTemplate("${ConfirmBooking()}"),
                    // You can use this flag to control when a specific input participates in consultation bubbling and can be interrupted.
                    // 'false' means intteruption is not allowed when this input is active.
                    AllowInterruptions = "false"
                },
                new IfCondition()
                {
                    // All conditions are expressed using adaptive expressions.
                    // See https://aka.ms/adaptive-expressions to learn more
                    Condition = "turn.bookingConfirmation == true",
                    Actions = new List<Dialog>()
                    {
                        // TODO: book flight.
                        new SendActivity("${BookingConfirmation()}")
                    },
                    ElseActions = new List<Dialog>() {new SendActivity("Thank you.")}
                }
            };
        }
    }
}
