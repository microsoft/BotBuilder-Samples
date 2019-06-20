using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.LanguageGeneration;

/// <summary>
/// TODO
/// 1. Accept meeting
///     accepting meeting is possible for each meeting or creating the only one focused meeting is our task
///     // each one has one option of accepting // if show me the first one, then give the user an option to accept it
/// 2. decline meeting
/// 3. update meeting
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : ComponentDialog
    {
        private static IConfiguration Configuration;

        public RootDialog(IConfiguration configuration)
            : base(nameof(RootDialog))
        {
            Configuration = configuration;
            // Create instance of adaptive dialog. 
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // Create a LUIS recognizer.
                // The recognizer is built using the intents, utterances, patterns and entities defined in ./RootDialog.lu file
                Recognizer = CreateRecognizer(),
                Generator = new ResourceMultiLanguageGenerator("RootDialog.lg"),
                Rules = new List<IRule>()
                {
                    // Intent rules for the LUIS model. Each intent here corresponds to an intent defined in ./Dialogs/Resources/ToDoBot.lu file
                    new IntentRule("Greeting")
                    {
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[Help-Root-Dialog]")
                        },
                        Constraint = "turn.dialogevent.value.intents.Greeting.score > 0.6"
                    },

                    /******************************************************************************/
                    // place to add new dialog
                    new IntentRule("CreateCalendarEntry")
                    {
                        Steps = new List<IDialog>()
                        {
                            new BeginDialog(nameof(CreateCalendarEntry))
                        },
                        Constraint = "turn.dialogevent.value.intents.CreateCalendarEntry.score > 0.4"
                    },
                    new IntentRule("FindCalendarEntry")
                    {
                        Steps = new List<IDialog>()
                        {
                            new BeginDialog(nameof(FindCalendarEntry))
                        },
                        Constraint = "turn.dialogevent.value.intents.FindCalendarEntry.score > 0.4"
                    },
                    new IntentRule("DeleteCalendarEntry")
                    {
                        Steps = new List<IDialog>()
                        {
                            new BeginDialog(nameof(DeleteCalendarEntry))
                        },
                        Constraint = "turn.dialogevent.value.intents.DeleteCalendarEntry.score > 0.4"
                    },
                    new IntentRule("FindCalendarWho")
                    {
                        Steps = new List<IDialog>()
                        {
                            new BeginDialog(nameof(FindCalendarWho))
                        },
                        Constraint = "turn.dialogevent.value.intents.FindCalendarWho.score > 0.4"
                    },
                    /******************************************************************************/

                    // Come back with LG template based readback for global help
                    new IntentRule("Help")
                    {
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[Help-Root-Dialog]")
                        },
                        Constraint = "turn.dialogevent.value.intents.Help.score > 0.6"
                    },
                    new IntentRule("Cancel")
                    {
                        Steps = new List<IDialog>()
                        {
                            // This is the global cancel in case a child dialog did not explicit handle cancel.
                            new SendActivity("Cancelling all dialogs.."),
                            new SendActivity("[Welcome-Actions]"),
                            new CancelAllDialogs(),
                        },
                        Constraint = "turn.dialogevent.value.intents.Cancel.score > 0.6"
                    }
                }
            };

            /******************************************************************************/
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);
            rootDialog.AddDialog(new List<Dialog>()
            {
                new CreateCalendarEntry(),
                new FindCalendarEntry(),
                new DeleteCalendarEntry(),
                new FindCalendarWho()
            });
            /******************************************************************************/
            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
}


public static IRecognizer CreateRecognizer()
{
    if (string.IsNullOrEmpty(Configuration["LuisAppId"]) || string.IsNullOrEmpty(Configuration["LuisAPIKey"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostName"]))
    {
        throw new Exception("Your LUIS application is not configured. Please see README.MD to set up a LUIS application.");
    }
    return new LuisRecognizer(new LuisApplication()
    {
        Endpoint = Configuration["LuisAPIHostName"],
        EndpointKey = Configuration["LuisAPIKey"],
        ApplicationId = Configuration["LuisAppId"]
    });
}
    }
}
