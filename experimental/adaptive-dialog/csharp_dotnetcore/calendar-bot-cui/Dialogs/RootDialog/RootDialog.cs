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
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;

/// <summary>
/// This dialog is the lowest level of all dialogs. 
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
            var rootDialog = new AdaptiveDialog("root")
            {
                // Create a LUIS recognizer.
                // The recognizer is built using the intents, utterances, patterns and entities defined in ./RootDialog.lu file
                Recognizer = CreateRecognizer(),
                Generator = new ResourceMultiLanguageGenerator("RootDialog.lg"),
                Steps = new List<IDialog>() {
                    new TextInput()
                    {
                        Property =  "turn.dump",
                        Prompt = new ActivityTemplate("[Help-Root-Dialog]")
                    }
                },
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
                            new SetProperty()
                            {
                                Property = "user.CreateCalendarEntry_pageIndex", // 0-based
                                Value = "0"
                            },
                            new SetProperty()
                            {
                                Property = "user.CreateInput",
                                Value = "true"
                            },
                            new BeginDialog(nameof(CreateCalendarEntry))
                        },
                        Constraint = "turn.dialogevent.value.intents.CreateCalendarEntry.score > 0.5"
                    },
                    new IntentRule("FindCalendarEntry")
                    {
                        Steps = new List<IDialog>()
                        {
                            new SetProperty()
                            {
                                Property = "user.FindCalendarEntry_pageIndex",// 0-based
                                Value = "0"
                            },
                            new BeginDialog(nameof(FindCalendarEntry))
                        },
                        Constraint = "turn.dialogevent.value.intents.FindCalendarEntry.score > 0.5"
                    },
                    new IntentRule("DeleteCalendarEntry")
                    {
                        Steps = new List<IDialog>()
                        {
                            new BeginDialog(nameof(DeleteCalendarEntry))
                        },
                        Constraint = "turn.dialogevent.value.intents.DeleteCalendarEntry.score > 0.5"
                    },
                    new IntentRule("FindCalendarWho")
                    {
                        Steps = new List<IDialog>()
                        {
                             new SetProperty()
                            {
                                Property = "user.FindCalendarWho_pageIndex",// 0-based
                                Value = "0"
                            },
                             new SetProperty()
                            {
                                Property = "user.WhoInput",
                                Value = "true"
                            },
                            new BeginDialog(nameof(FindCalendarWho))
                        },
                        Constraint = "turn.dialogevent.value.intents.FindCalendarWho.score > 0.5"
                    },
                    new IntentRule("AcceptCalendarEntry")
                    {
                        Steps = new List<IDialog>()
                        {
                            new BeginDialog(nameof(AcceptCalendarEntry))
                        },
                        Constraint = "turn.dialogevent.value.intents.AcceptCalendarEntry.score > 0.5"
                    },
                    new IntentRule("ShowNextCalendar"){
                        Steps = new List<IDialog>()
                        {
                            new BeginDialog(nameof(ShowNextCalendar))
                        },
                        Constraint = "turn.dialogevent.value.intents.ShowNextCalendar.score > 0.5"
                    },
                    new IntentRule("ChangeCalendarEntry"){
                        Steps = new List<IDialog>()
                        {
                            new BeginDialog(nameof(ChangeCalendarEntry))
                        },
                        Constraint = "turn.dialogevent.value.intents.ChangeCalendarEntry.score > 0.5"
                    },
                    /******************************************************************************/

                    // Come back with LG template based readback for global help
                    new IntentRule("Help")
                    {
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[Help-Root-Dialog]")
                        },
                        Constraint = "turn.dialogevent.value.intents.Help.score > 0.5"
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
                        Constraint = "turn.dialogevent.value.intents.Cancel.score > 0.5"
                    }
                }
            };

            /******************************************************************************/
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);
            rootDialog.AddDialog(new List<Dialog>()
            {
                new CreateCalendarEntry(Configuration),
                new FindCalendarEntry(Configuration),
                new DeleteCalendarEntry(),
                new FindCalendarWho(),
                new AcceptCalendarEntry(),
                new OAuthPromptDialog(),
                new ShowNextCalendar(),
                new ChangeCalendarEntry()
            }) ;
            /******************************************************************************/
            // The initial child Dialog to run.
            InitialDialogId = "root";
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
