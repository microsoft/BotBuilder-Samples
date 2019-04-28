using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RootDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;

        public RootDialog(IConfiguration configuration)
           : base(nameof(RootDialog))
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
                // The intents here are based on intents defined in MainAdaptiveDialog.LU file
                new IntentRule("Book_flight")
                {
                    Steps = new List<IDialog>()
                    {
                        // Save any entities returned by the LUIS recognizer
                        // @entityName is a short-hand to turn.entities.<entityName>
                        // Other short hands include -
                        //    - #intentName is a short-hand for turn.intents.<IntentName>
                        //    - $propertyName is a short-hand for dialog.results.<propertyName>
                        //new CodeStep(testCodeStep),
                        new SaveEntity("dialog.flightBooking.destinationCity", "@geographyV2[0]"),
                        new SaveEntity("dialog.flightBooking.departureDate", "@datetimeV2[0]"),
                        // Steps to book flight
                        // Help and Cancel intents are always available since TextInput will always initiate
                        // Consulatation up the parent dialog chain to see if anyone else wants to take the user input.
                        new TextInput()
                        {
                            Property = "dialog.flightBooking.destinationCity",
                            Prompt = new ActivityTemplate("[PromptForMissingInformation]")
                        },
                        new TextInput()
                        {
                            Property = "dialog.flightBooking.departureCity",
                            Prompt = new ActivityTemplate("[PromptForMissingInformation]")
                        },
                        new TextInput()
                        {
                            Property = "dialog.flightBooking.departureDate",
                            Prompt = new ActivityTemplate("[PromptForMissingInformation]")
                        },
                        new ConfirmInput()
                        {
                            Property = "turn.confirmation",
                            Prompt = new ActivityTemplate("[ConfirmBooking]")
                        },
                        new IfCondition()
                        {
                            Condition = new ExpressionEngine().Parse("turn.confirmation == true"),
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
                },
                // This intent could fire when we are in the middle of book flight conversation.
                // Just save any entities we might have recieved from LUIS and move forward with current conversation.
                new IntentRule("GetMissingEntitiesForFlight")
                {
                    Steps = new List<IDialog>()
                    {
                        new LogStep()
                        {
                            Text = new TextTemplate(new ActivityTemplate("[Log-Trace]").ToString()),
                            TraceActivity = true
                        },
                        // See if we have a geography entity. Confirm with user if they would like to use this as 
                        // Departure or destination city.
                        new IfCondition()
                        {
                            Condition = new ExpressionEngine().Parse("@geographyV2 != null && (dialog.flightBooking.destinationCity != null || dialog.flightBooking.departureCity != null)"),
                            Steps = new List<IDialog>()
                            {
                                new ChoiceInput()
                                {
                                    Property = "turn.choiceOutcome",
                                    Prompt = new ActivityTemplate("[Disambiguate-city]"),
                                    Style = ListStyle.Auto,
                                    Choices = new List<Choice>()
                                    {
                                        new Choice("Departure city"),
                                        new Choice("Destination city")
                                    }
                                },
                                new SwitchCondition()
                                {
                                    Condition = "turn.choiceOutcome",
                                    Cases = new List<Case>()
                                    {
                                        new Case("Departure city") { Steps = new List<IDialog>() { new SaveEntity("dialog.flightBooking.departureCity", "@geographyV2[0]") } },
                                        new Case("Destination city") { Steps = new List<IDialog>() { new SaveEntity("dialog.flightBooking.departureDate", "@datetimeV2[0]") } }
                                    }
                                }
                            },
                            ElseSteps = new List<IDialog>()
                            {
                                 new LogStep()
                                 {
                                    Text = new TextTemplate(new ActivityTemplate("[Log-Trace]").ToString()),
                                    TraceActivity = true
                                 },
                                new SaveEntity("dialog.flightBooking.departureCity", "@geographyV2[0]")
                            }
                        },
                        // Set any value received for datetime as departure date
                        new SaveEntity("dialog.flightBooking.departureDate", "@datetimeV2[0]")
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
                        new SendActivity("[BotOverview]")
                    }
                },
                new IntentRule("Greeting")
                {
                    Steps = new List<IDialog>()
                    {
                        new SendActivity("[BotOverview]")
                    }
                },
                new IntentRule("None")
                {
                    Steps = new List<IDialog>()
                    {
                        new SendActivity("I'm sorry, I do not understand that. Can you please rephrase what you are saying?"),
                        new SendActivity("[BotOverview]")
                    }
                }
            };
        }
    }       
}
