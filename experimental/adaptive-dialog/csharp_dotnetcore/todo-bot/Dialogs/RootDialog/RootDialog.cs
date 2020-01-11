using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using System.IO;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : ComponentDialog
    {
        private static IConfiguration Configuration;

        public RootDialog(IConfiguration configuration)
            : base(nameof(RootDialog))
        {
            Configuration = configuration;
            string[] paths = { ".", "Dialogs", "RootDialog", "RootDialog.lg" };
            string fullPath = Path.Combine(paths);
            // Create instance of adaptive dialog. 
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // Add a generator. This is how all Language Generation constructs specified for this dialog are resolved.
                Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(fullPath)),
                // Create a LUIS recognizer.
                // The recognizer is built using the intents, utterances, patterns and entities defined in ./RootDialog.lu file
                Recognizer = CreateRecognizer(),
                Triggers = new List<OnCondition>()
                {
                    // Add a rule to welcome user
                    new OnConversationUpdateActivity()
                    {
                        Actions = WelcomeUserSteps()
                    },
                    // Intent rules for the LUIS model. Each intent here corresponds to an intent defined in ./Dialogs/Resources/ToDoBot.lu file
                    new OnIntent("Greeting")         
                    { 
                        Actions = new List<Dialog>() 
                        { 
                            new SendActivity("@{Help-Root-Dialog()}") 
                            } 
                    },
                    new OnIntent("AddToDoDialog")    
                    { 
                        // LUIS returns a confidence score with intent classification. 
                        // Conditions are expressions. 
                        // This expression ensures that this trigger only fires if the confidence score for the 
                        // AddToDoDialog intent classification is at least 0.7
                        Condition = "#AddToDoDialog.Score >= 0.5",
                        Actions = new List<Dialog>() 
                        { 
                            new BeginDialog(nameof(AddToDoDialog)) 
                        } 
                    },
                    new OnIntent("DeleteToDoDialog") 
                    { 
                        Condition = "#DeleteToDoDialog.Score >= 0.5",
                        Actions = new List<Dialog>() 
                        { 
                            new BeginDialog(nameof(DeleteToDoDialog)) 
                        } 
                    },
                    new OnIntent("ViewToDoDialog")   
                    { 
                        Condition = "#ViewToDoDialog.Score >= 0.5",
                        Actions = new List<Dialog>() 
                        { 
                            new BeginDialog(nameof(ViewToDoDialog)) 
                        } 
                    },
                    // Come back with LG template based readback for global help
                    new OnIntent("Help")             
                    { 
                        Condition = "#Help.Score >= 0.8",
                        Actions = new List<Dialog>() 
                        { 
                            new SendActivity("@{Help-Root-Dialog()}") 
                        } 
                    },
                    new OnIntent("Cancel")           
                    { 
                        Condition = "#Cancel.Score >= 0.8",
                        Actions = new List<Dialog>() 
                        {
                            // This is the global cancel in case a child dialog did not explicit handle cancel.
                            new SendActivity("Cancelling all dialogs.."),
                            // SendActivity supports full language generation resolution.
                            // See here to learn more about language generation
                            // https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/language-generation
                            new SendActivity("@{Welcome-Actions()}"),
                            new CancelAllDialogs(),
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);

            // Add all child dialogS
            AddDialog(new AddToDoDialog());
            AddDialog(new DeleteToDoDialog());
            AddDialog(new ViewToDoDialog());

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
                                new SendActivity("@{Intro-message()}")
                            }
                        }
                    }
                }
            };
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
