using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainAdaptiveDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;

        public MainAdaptiveDialog(IConfiguration configuration)
           : base(nameof(MainAdaptiveDialog))
        {
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
                Rules = CreateRules()
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
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

        private static List<IRule> CreateRules()
        {
            return new List<IRule>()
            {
                // The intents here are based on intents defined in FlightBooking.LU file under the Resources folder
                new IntentRule("Book_flight")
                {
                    Steps = new List<IDialog>()
                    {
                        // Save any entities returned by the LUIS recognizer
                        // @entityName is a short-hand to turn.entities.<entityName>
                        // Other short hands include -
                        //    - #intentName is a short-hand for turn.intents.<IntentName>
                        //    - $propertyName is a short-hand for dialog.results.<propertyName>
                        new SaveEntity("dialog.flightBooking.destinationCity", "@ToCity"),
                        new SaveEntity("dialog.flightBooking.departureCity", "@FromCity"),
                        new SaveEntity("dialog.flightBooking.departureDate", "@datetimeV2"),
                        // Steps to book flight
                        // Help and Cancel intents are always available since TextInput will always initiate
                        // Consulatation up the parent dialog chain to see if anyone else wants to take this input.
                        new TextInput()
                        {
                            Property = "dialog.flightBooking.destinationCity",
                            Prompt = new ActivityTemplate("What is your destination city?")
                        },
                        new TextInput()
                        {
                            Property = "dialog.flightBooking.departureCity",
                            Prompt = new ActivityTemplate("What is your departure city?")
                        },
                        new TextInput()
                        {
                            Property = "dialog.flightBooking.departureDate",
                            Prompt = new ActivityTemplate("What is your departure date?")
                        },
                        new ConfirmInput()
                        {
                            Property = "turn.confirmation",
                            Prompt = new ActivityTemplate("[FlightBookingReadBack")
                        },
                        new IfCondition()
                        {
                            Condition = new ExpressionEngine().Parse("turn.confirmation == true"),
                            Steps = new List<IDialog>()
                            {
                                new SendActivity("Your flight is confirmed!")
                            },
                            ElseSteps = new List<IDialog>()
                            {
                                new SendActivity("Thank you.")
                            }
                        },
                        new EndDialog()
                    }
                },
                new IntentRule("Cancel")
                {
                    Steps = new List<IDialog>()
                    {
                        new SendActivity("Sure, cancelling that..."),
                        new CancelAllDialogs(),
                        new EndDialog()
                    }
                },
                new IntentRule("Help")
                {
                    Steps = new List<IDialog>()
                    {
                        new SendActivity("Hello, I'm the core bot! ")
                        //new SendActivity("[BotOverview]")
                    }
                },
                new IntentRule("None")
                {
                    Steps = new List<IDialog>()
                    {
                        new SendActivity("I'm sorry, I do not understand that. Can you please rephrase what you are saying?"),
                        // TODO: Add LG template with overview card and suggested actions. 
                        //new SendActivity("[BotOverview]")
                    }
                }
            };
        }
    }       
}
