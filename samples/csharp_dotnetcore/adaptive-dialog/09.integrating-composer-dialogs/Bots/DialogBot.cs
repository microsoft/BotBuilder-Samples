// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    // This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
    // to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
    // each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
    // The ConversationState is used by the Dialog system. The UserState isn't, however, it might have been used in a Dialog implementation,
    // and the requirement is that all BotState objects are saved at the end of a turn.
    public class DialogBot<T> : ActivityHandler 
        where T : Dialog
    {
        private readonly ILogger Logger;
        private readonly DialogManager DialogManager;

        public DialogBot(ResourceExplorer resourceExplorer, T dialog, ILogger<DialogBot<T>> logger)
        {
            Logger = logger;
            DialogManager = new DialogManager(dialog);
            DialogManager.UseResourceExplorer(resourceExplorer);
            DialogManager.UseLanguageGeneration();
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            Logger.LogInformation("Running dialog with OnTurn");
            await DialogManager.OnTurnAsync(turnContext, cancellationToken);
        }
    }
}
