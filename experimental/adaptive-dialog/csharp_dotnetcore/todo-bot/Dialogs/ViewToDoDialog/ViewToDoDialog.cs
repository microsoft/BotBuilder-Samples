using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;

namespace Microsoft.BotBuilderSamples
{
    public class ViewToDoDialog : ComponentDialog
    {
        public ViewToDoDialog()
            : base(nameof(ViewToDoDialog))
        {
            // Create instance of adaptive dialog. 
            var ViewToDoDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Steps = new List<IDialog>()
                {
                    new SendActivity("[View-ToDos]")
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(ViewToDoDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
