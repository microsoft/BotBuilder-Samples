// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Bot.Builder.Skills;

namespace RunBotServer
{
    public class RunBot : ActivityHandler
    {
        private IStatePropertyAccessor<DialogState> dialogStateAccessor;
        private DialogManager dialogManager;
        private readonly ResourceExplorer resourceExplorer;

        public RunBot(ConversationState conversationState, ResourceExplorer resourceExplorer, BotFrameworkClient skillClient, SkillConversationIdFactoryBase conversationIdFactory)
        {
            this.dialogStateAccessor = conversationState.CreateProperty<DialogState>("RootDialogState");
            this.resourceExplorer = resourceExplorer;

            // auto reload dialogs when file changes
            this.resourceExplorer.Changed += (sender, resources) =>
            {
                if (resources.Any(resource => resource.Id.EndsWith(".dialog") || resource.Id.EndsWith(".lg")))
                {
                    Task.Run(() => this.LoadDialogs());
                }
            };
            LoadDialogs();
        }

        public override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.dialogManager.OnTurnAsync(turnContext, cancellationToken: cancellationToken);
        }

        private void LoadDialogs()
        {
            System.Diagnostics.Trace.TraceInformation("Loading resources...");

            var rootDialog = new AdaptiveDialog()
            {
                AutoEndDialog = false,
            };
            var choiceInput = new ChoiceInput()
            {
                Prompt = new ActivityTemplate("What dialog do you want to run?"),
                Property = "conversation.dialogChoice",
                AlwaysPrompt = true,
                Choices = new ChoiceSet(new List<Choice>())
            };

            var handleChoice = new SwitchCondition()
            {
                Condition = "conversation.dialogChoice",
                Cases = new List<Case>()
            };

            Dialog lastDialog = null;
            var choices = new ChoiceSet();

            foreach (var resource in this.resourceExplorer.GetResources(".dialog").Where(r => r.Id.EndsWith(".main.dialog")))
            {
                try
                {
                    var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(resource.Id));
                    choices.Add(new Choice(name));
                    var dialog = resourceExplorer.LoadType<Dialog>(resource);
                    lastDialog = dialog;
                    handleChoice.Cases.Add(new Case($"{name}", new List<Dialog>() { dialog }));
                }
                catch (SyntaxErrorException err)
                {
                    Trace.TraceError($"{err.Source}: Error: {err.Message}");
                }
                catch (Exception err)
                {
                    Trace.TraceError(err.Message);
                }
            }

            if (handleChoice.Cases.Count() == 1)
            {
                rootDialog.Triggers.Add(new OnBeginDialog
                {
                    Actions = new List<Dialog>
                    {
                        lastDialog,
                        new RepeatDialog()
                    }
                });
            }
            else
            {
                choiceInput.Choices = choices;
                choiceInput.Style = ListStyle.Auto;
                rootDialog.Triggers.Add(new OnBeginDialog()
                {
                    Actions = new List<Dialog>()
                {
                    choiceInput,
                    new SendActivity("# Running ${conversation.dialogChoice}.main.dialog"),
                    handleChoice,
                    new RepeatDialog()
                }
                });
            }

            this.dialogManager = new DialogManager(rootDialog)
                .UseResourceExplorer(this.resourceExplorer)
                .UseLanguageGeneration();

            Trace.TraceInformation("Done loading resources.");
        }
    }
}
