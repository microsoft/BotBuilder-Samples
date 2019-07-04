using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;

/// <summary>
/// This dialog will prompt user to log into calendar account
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class OAuthPromptDialog : ComponentDialog
    {
        public OAuthPromptDialog()
            : base(nameof(OAuthPromptDialog))
        {
            var oAuthPromptDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Steps = new List<IDialog>()
                {
                    new OAuthPrompt("OAuthPrompt",
                        new OAuthPromptSettings()
                        {
                            Text = "Please log in to your calendar account",
                            ConnectionName = "msgraph",
                            Title = "Sign in",
                        }
                    ){
                        Property = "user.token"
                    }                  
                }
            };
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(oAuthPromptDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}

