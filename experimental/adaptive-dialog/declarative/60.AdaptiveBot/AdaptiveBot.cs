// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Debugging;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Bot.Builder.Dialogs.Declarative.Types;
using Microsoft.Bot.Schema;
using static Microsoft.Bot.Builder.Dialogs.Debugging.Source;

namespace Microsoft.BotBuilderSamples
{
    public class AdaptiveBot : ActivityHandler
    {
        private IStatePropertyAccessor<DialogState> dialogStateAccessor;
        private AdaptiveDialog rootDialog;
        private readonly ResourceExplorer resourceExplorer;

        public AdaptiveBot(ConversationState conversationState, ResourceExplorer resourceExplorer)
        {
            this.dialogStateAccessor = conversationState.CreateProperty<DialogState>("RootDialogState");
            this.resourceExplorer = resourceExplorer;

            // auto reload dialogs when file changes
            this.resourceExplorer.Changed += (paths) =>
            {
                if (paths.Any(p => Path.GetExtension(p) == ".dialog"))
                {
                    Task.Run(() => this.LoadDialogs());
                }
            };

            LoadDialogs();
        }


        private void LoadDialogs()
        {
            System.Diagnostics.Trace.TraceInformation("Loading resources...");

            // normally you just look up your root dialog and load it
            //var resource = this.resourceExplorer.GetResource("myroot.dialog");
            //this.rootDialog = DeclarativeTypeLoader.Load<AdaptiveDialog>(resource, this.resourceExplorer, DebugSupport.SourceRegistry);

            // but for this sample we enumerate all of the .main.dialog files and build a ChoiceInput as our rootidialog.
            this.rootDialog = CreateChoiceInputForAllMainDialogs();

            System.Diagnostics.Trace.TraceInformation("Done loading resources.");
        }

        private AdaptiveDialog CreateChoiceInputForAllMainDialogs()
        {
            var dialog = new AdaptiveDialog()
            {
                AutoEndDialog = false,
                Steps = new List<IDialog>()
            };
            var choiceInput = new ChoiceInput()
            {
                Prompt = new ActivityTemplate("What declarative sample do you want to run?"),
                OutputBinding = "conversation.dialogChoice",
                AlwaysPrompt = true,
                Choices = new List<Choice>()
            };

            var handleChoice = new SwitchCondition()
            {
                Condition = "conversation.dialogChoice",
                Cases = new List<Case>()
            };

            foreach (var resource in this.resourceExplorer.GetResources(".dialog").Where(r => r.Id.EndsWith(".main.dialog")))
            {
                var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(resource.Id));
                choiceInput.Choices.Add(new Choice(name));
                var subDialog = DeclarativeTypeLoader.Load<IDialog>(resource, this.resourceExplorer, DebugSupport.SourceRegistry);
                handleChoice.Cases.Add(new Case($"'{name}'", new List<IDialog>() { subDialog }));
            }
            choiceInput.Style = ListStyle.Auto;
            dialog.Steps.Add(choiceInput);
            dialog.Steps.Add(new SendActivity("# Running {conversation.dialogChoice}.main.dialog"));
            dialog.Steps.Add(handleChoice);
            dialog.Steps.Add(new RepeatDialog());
            return dialog;
        }

        protected override Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return rootDialog.OnTurnAsync((ITurnContext)turnContext, null, cancellationToken);
        }

        protected async override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await rootDialog.OnTurnAsync(turnContext, null, cancellationToken).ConfigureAwait(false);
        }
    }
}
