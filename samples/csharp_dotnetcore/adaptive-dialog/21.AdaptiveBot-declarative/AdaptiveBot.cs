// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;

namespace Microsoft.BotBuilderSamples
{
    public class AdaptiveBot : ActivityHandler
    {
        private IStatePropertyAccessor<DialogState> dialogStateAccessor;
        //private AdaptiveDialog rootDialog;
        private DialogManager dialogManager;
        private readonly ResourceExplorer resourceExplorer;

        public AdaptiveBot(ConversationState conversationState, ResourceExplorer resourceExplorer)
        {
            this.dialogStateAccessor = conversationState.CreateProperty<DialogState>("RootDialogState");
            this.resourceExplorer = resourceExplorer;

            // auto reload dialogs when file changes
            this.resourceExplorer.Changed += (e, resources) =>
            {
                if (resources.Any(resource => resource.Id.EndsWith(".dialog")))
                {
                    Task.Run(() => this.LoadDialogs());
                }
            };
            LoadDialogs();
        }


        private void LoadDialogs()
        {
            System.Diagnostics.Trace.TraceInformation("Loading resources...");

            //For this sample we enumerate all of the .main.dialog files and build a ChoiceInput as our rootidialog.
            this.dialogManager = new DialogManager(CreateChoiceInputForAllMainDialogs());
            this.dialogManager.UseResourceExplorer(this.resourceExplorer);
            this.dialogManager.UseLanguageGeneration();

            System.Diagnostics.Trace.TraceInformation("Done loading resources.");
        }

        private AdaptiveDialog CreateChoiceInputForAllMainDialogs()
        {
            var dialogChoices = new List<Choice>();
            var dialogCases = new List<Case>();
            foreach (var resource in this.resourceExplorer.GetResources(".dialog").Where(r => r.Id.EndsWith(".main.dialog")))
            {
                var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(resource.Id));
                dialogChoices.Add(new Choice(name));
                var subDialog = resourceExplorer.LoadType<AdaptiveDialog>(resource);
                dialogCases.Add(new Case($"{name}", new List<Dialog>() { subDialog }));
            }

            var dialog = new AdaptiveDialog()
            {
                AutoEndDialog = false,
                Triggers = new List<OnCondition>() {
                    new OnBeginDialog() {
                        Actions = new List<Dialog>() {
                            new ChoiceInput() {
                                Prompt = new ActivityTemplate("What declarative sample do you want to run?"),
                                Property = "conversation.dialogChoice",
                                AlwaysPrompt = true,
                                Style = ListStyle.List,
                                Choices = new ChoiceSet(dialogChoices)
                            },
                            new SendActivity("# Running ${conversation.dialogChoice}.main.dialog"),
                            new SwitchCondition(){
                                Condition = "conversation.dialogChoice",
                                Cases = dialogCases
                            },
                            new RepeatDialog()
                        }
                    }
                }
            };
            return dialog;
        }

        public override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.dialogManager.OnTurnAsync(turnContext, cancellationToken: cancellationToken);
        }
    }
}
