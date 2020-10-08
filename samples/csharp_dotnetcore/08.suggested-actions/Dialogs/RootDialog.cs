using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : AdaptiveDialog
    {
        private readonly Templates _templates;
        public RootDialog() : base(nameof(RootDialog))
        {
            string[] paths = { ".", "Dialogs", "RootDialog.lg" };
            var fullPath = Path.Combine(paths);
            _templates = Templates.ParseFile(fullPath);

            Triggers = new List<OnCondition>
            {
                new OnConversationUpdateActivity()
                {
                    Actions = WelcomeUserSteps()
                },
                new OnEndOfActions()
                {
                    Actions = GetSuggestedActions()
                }
            };

            Generator = new TemplateEngineLanguageGenerator(_templates);

        }

        private static List<Dialog> GetSuggestedActions() =>
            new List<Dialog>()
            {
                new TextInput()
                {
                    Property = "turn.userChoice",
                    Prompt = new ActivityTemplate("${ColorChoices()}")
                },
                new IfCondition()
                {
                    Condition = "turn.userChoice == 'Cancel'",
                    Actions = new List<Dialog>
                    {
                        new SendActivity("${CancelMessage()}"),
                        new EndDialog()
                    }
                },
                new SendActivity("${ColorChosenReply(turn.userChoice)}"),
                new DeleteProperty(){ Property = "turn.userChoice"}
            };

        private static List<Dialog> WelcomeUserSteps() => new List<Dialog>()
            {
                new Foreach()
                {
                    ItemsProperty = "turn.activity.membersAdded",
                    Actions = new List<Dialog>()
                    {
                        new IfCondition()
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>()
                            {
                                new SendActivity("${WelcomeMessage()}"),
                            }
                        },
                    }
                },
            };
    }

}
