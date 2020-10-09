using System.Collections.Generic;
using System.IO;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RootDialog : AdaptiveDialog
    {
        private readonly Templates _templates;

        public RootDialog() : base(nameof(RootDialog))
        {
            string[] paths = { ".", "Dialogs", $"RootDialog.lg" };
            var fullPath = Path.Combine(paths);
            _templates = Templates.ParseFile(fullPath);

            Triggers = new List<OnCondition>
            {
                // Add a rule to welcome user
                new OnConversationUpdateActivity()
                {
                    Actions = WelcomeUserSteps()
                },

                new OnUnknownIntent
                {
                    Actions = GetUserCardChoice()
                }
            };

            Generator = new TemplateEngineLanguageGenerator(_templates);
        }

        private List<Dialog> GetUserCardChoice()
        {
            return new List<Dialog>()
            {
                new ChoiceInput()
                {
                    // Output from the user is automatically set to this property
                    Property = "turn.cardChoice",

                    // List of possible styles supported by choice prompt.
                    Style = Bot.Builder.Dialogs.Choices.ListStyle.Auto,
                    Prompt = new ActivityTemplate("${CardChoicePrompt()}"),
                    Choices = new ObjectExpression<ChoiceSet>(_templates.Evaluate("CardChoices") as string),
                },
                new SwitchCondition()
                {
                    Condition = "turn.cardChoice",
                    Cases = new List<Case>() {
                        new Case("Adaptive Card",  new List<Dialog>() { new SendActivity("${AdaptiveCard()}") } ),
                        new Case("Animation Card", new List<Dialog>() { new SendActivity("${AnimationCard()}") } ),
                        new Case("Audio Card",     new List<Dialog>() { new SendActivity("${AudioCard()}") } ),
                        new Case("Hero Card",      new List<Dialog>() { new SendActivity("${HeroCard()}") } ),
                        new Case("Signin Card",    new List<Dialog>() { new SendActivity("${SigninCard()}") } ),
                        new Case("Thumbnail Card", new List<Dialog>() { new SendActivity("${ThumbnailCard()}") } ),
                        new Case("Video Card",     new List<Dialog>() { new SendActivity("${VideoCard()}") } ),
                        new Case("Cancel",         new List<Dialog>() { new SendActivity("Sure."), new EndDialog() } ),
                    },
                    Default = new List<Dialog>()
                    {
                        new SendActivity("${AllCards()}")
                    }
                },
                new SendActivity("${CardStartOverResponse()}"),
            };
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
                        new IfCondition()
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>()
                            {
                                new SendActivity("${WelcomeMessage()}")
                            }
                        }
                    }
                }
            };
        }
    }
}
