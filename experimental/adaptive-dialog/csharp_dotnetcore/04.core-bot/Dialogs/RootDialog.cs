using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RootDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        private TemplateEngine _lgEngine;

        public RootDialog(IConfiguration configuration)
           : base(nameof(RootDialog))
        {
            Configuration = configuration;
            _lgEngine = new TemplateEngine().AddFile(Path.Combine(".", "Dialogs", "RootDialog.lg"));
            
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
                Generator = new TemplateEngineLanguageGenerator(_lgEngine),
                Rules = new List<IRule>()
                {
                    // Add a rule to welcome user
                    new ConversationUpdateActivityRule()
                    {
                        Steps = WelcomeUserSteps()
                    },
                    // Add additional rules to respond to other intents returned by the LUIS application.
                    // The intents here are based on intents defined in MainAdaptiveDialog.LU file
                    new IntentRule()
                    {
                        Intent = "Cancel",
                        Constraint = "turn.recognized.intents.Cancel.score > 0.5",
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("Sure, cancelling that..."),
                            new CancelAllDialogs(),
                            new EndDialog()
                        }
                    },
                    new IntentRule()
                    {
                        Intent = "Help",
                        Steps = new List<IDialog> ()
                        {
                            new SendActivity("[BotOverview]")
                        }
                    },
                    new IntentRule()
                    {
                        Intent = "Greeting",
                        Steps = new List<IDialog> ()
                        {
                            new SendActivity("[BotOverview]")
                        }
                    },
                    new IntentRule("Book_flight")
                    {
                        Steps = new List<IDialog>()
                        {
                            // Save any entities returned by LUIS.
                            // We will only save any geography city entities that explicitly have been classified as either fromCity or toCity.
                            new SetProperty()
                            {
                                Property = "conversation.flightBooking.departureCity",
                                Value = "turn.recognized.entities.fromCity[0].geographyV2_city[0]"
                            },
                            new SetProperty()
                            {
                                Property = "conversation.flightBooking.destinationCity",
                                Value = "turn.recognized.entities.toCity[0].geographyV2_city[0]"
                            },
                            new SetProperty()
                            {
                                Property = "conversation.flightBooking.departureDate",
                                Value = "turn.recognized.entities.datetimeV2[0]"
                            },
                            // Steps to book flight
                            // Help and Cancel intents are always available since TextInput will always initiate
                            // Consulatation up the parent dialog chain to see if anyone else wants to take the user input.
                            new TextInput()
                            {
                                Property = "conversation.flightBooking.departureCity",
                                // Prompt property supports full language generation resolution.
                                // See here to learn more about language generation
                                // https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/language-generation
                                Prompt = new ActivityTemplate("[PromptForMissingInformation]"),
                                AllowInterruptions = AllowInterruptions.Never
                            },
                            new TextInput()
                            {
                                Property = "conversation.flightBooking.destinationCity",
                                Prompt = new ActivityTemplate("[PromptForMissingInformation]"),
                                AllowInterruptions = AllowInterruptions.Never
                            },
                            new DateTimeInput()
                            {
                                Property = "conversation.flightBooking.departureDate",
                                Prompt = new ActivityTemplate("[PromptForMissingInformation]"),
                                // You can use this flag to control when a specific input participates in consultation bubbling and can be interrupted.
                                // NotRecognized will only consult up to the parent dialog if the user input does not include a data time value in this case.
                                AllowInterruptions = AllowInterruptions.NotRecognized
                            },
                            new ConfirmInput()
                            {
                                Property = "turn.bookingConfirmation",
                                Prompt = new ActivityTemplate("[ConfirmBooking]")
                            },
                            new IfCondition()
                            {
                                // All conditions are expressed using the common expression language.
                                // See https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language to learn more
                                Condition = "turn.bookingConfirmation == true",
                                Steps = new List<IDialog>()
                                {
                                    // TODO: book flight.
                                    new SendActivity("[BookingConfirmation]")
                                },
                                ElseSteps = new List<IDialog>()
                                {
                                    new SendActivity("Thank you.")
                                }
                            }
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private static List<IDialog> WelcomeUserSteps()
        {
            return new List<IDialog>()
            {
                // Iterate through membersAdded list and greet user added to the conversation.
                new Foreach()
                {
                    ListProperty = "turn.activity.membersAdded",
                    ValueProperty = "turn.memberAdded",
                    Steps = new List<IDialog>()
                    {
                        // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                        // Filter cases where the bot itself is the recipient of the message. 
                        new IfCondition()
                        {
                            Condition = "turn.memberAdded.name != turn.activity.recipient.name",
                            Steps = new List<IDialog>()
                            {
                                new SendActivity("[WelcomeCard]")
                            }
                        }
                    }
                }
            };
        }

        private static IRecognizer CreateRecognizer(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["LuisAppId"]) || string.IsNullOrEmpty(configuration["LuisAPIKey"]) || string.IsNullOrEmpty(configuration["LuisAPIHostName"]))
            {
                throw new Exception("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.");
            }
            // Create the LUIS settings from configuration.
            var luisApplication = new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisAPIKey"],
                "https://" + configuration["LuisAPIHostName"]
            );

            return new LuisRecognizer(luisApplication);
        }
    }
}
