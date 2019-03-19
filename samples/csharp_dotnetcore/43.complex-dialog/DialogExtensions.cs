// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    public static class DialogExtensions
    {
        
        public static async Task Run(
            this Dialog dialog,
            ITurnContext turnContext,
            ConversationState conversationState,
            UserState userState,
            CancellationToken cancellationToken)
        {
            var userStateAccessor = userState.CreateProperty<UserProfile>("UserProfile");
            var conversationStateAccessor = conversationState.CreateProperty<DialogState>("DialogState");
            var dialogSet = new DialogSet(conversationStateAccessor);

            dialogSet.Add(dialog);

            var dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken);
            var results = await dialogContext.ContinueDialogAsync(cancellationToken);

            switch (results.Status)
            {
                case DialogTurnStatus.Cancelled:
                case DialogTurnStatus.Empty:
                    // If there is no active dialog, we should clear the user info and start a new dialog.
                    await userStateAccessor.SetAsync(turnContext, new UserProfile(), cancellationToken);
                    await userState.SaveChangesAsync(turnContext, false, cancellationToken);
                    await dialogContext.BeginDialogAsync(dialog.Id, null, cancellationToken);
                    break;
                case DialogTurnStatus.Complete:
                    // If we just finished the dialog, capture and display the results.
                    UserProfile userInfo = results.Result as UserProfile;
                    string status = "You are signed up to review "
                    + (userInfo.CompaniesToReview.Count is 0 ? "no companies" : string.Join(" and ", userInfo.CompaniesToReview))
                    + ".";

                    await turnContext.SendActivityAsync(status);
                    await userStateAccessor.SetAsync(turnContext, userInfo, cancellationToken);
                    await userState.SaveChangesAsync(turnContext, false, cancellationToken);
                    break;
                case DialogTurnStatus.Waiting:
                    // If there is an active dialog, we don't need to do anything here.
                    break;
            }
        }
    }
}
