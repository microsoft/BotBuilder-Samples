using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;

namespace Microsoft.BotBuilderSamples
{
    public class UserProfileDialog : ComponentDialog
    {
        public UserProfileDialog()
            : base(nameof(UserProfileDialog))
        {

            // Create instance of adaptive dialog. 
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // These steps are executed when this Adaptive Dialog begins
                Steps = OnBeginDialogSteps(),
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);
            
            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
        private static List<IDialog> OnBeginDialogSteps()
        {
            return new List<IDialog>()
            {
                // Ask for user's age and set it in user.userProfile scope.
                new TextInput()
                {
                    Prompt = new ActivityTemplate("Please enter your mode of transport."),
                    // Set the output of the text input to this property in memory.
                    Property = "user.userProfile.Transport"
                },
                new TextInput()
                {
                    Prompt = new ActivityTemplate("Please enter your name."),
                    Property = "user.userProfile.Name"
                },
                // Send activity supports full language generation. So you can refer to properties inline
                // or use pre-built functions inline or use language generation templates.
                new SendActivity("Thanks, {user.userProfile.Name}"),
                new ConfirmInput()
                {
                    Prompt = new ActivityTemplate("Would you like to give your age?"),
                    Property = "turn.ageConfirmation"
                },
                new IfCondition()
                {
                    Condition = new ExpressionEngine().Parse("turn.ageConfirmation == true"),
                    Steps = new List<IDialog>()
                    {
                         new NumberInput()
                         {
                             Prompt = new ActivityTemplate("Please enter your age."),
                             Property = "user.userProfile.Age",
                             // Set min and max constraints for age.
                             MinValue = 1,
                             MaxValue = 150,
                             // Specify a retry prompt if user input does not meet min or max constraint.
                             RetryPrompt = new ActivityTemplate("Sorry, that does not work. Your age must be greater than 0 and less than 150.")

                         },
                         new SendActivity("I have your age as {user.userProfile.Age}.")
                    },
                    ElseSteps = new List<IDialog>()
                    {
                        new SendActivity("No age given.") 
                    }
                },
                new ConfirmInput()
                {
                    Prompt = new ActivityTemplate("Is this ok?"),
                    Property = "turn.finalConfirmation"
                },
                // Use LG template to come back with the final read out.
                // This LG template is a great example of what logic can be wrapped up in LG sub-system.
                new SendActivity("[FinalUserProfileReadOut]"), // examines turn.finalConfirmation
                new EndDialog()
            };
        }
    }
}
