// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.authentication;

import java.util.Arrays;
import java.util.concurrent.CompletableFuture;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.prompts.ConfirmPrompt;
import com.microsoft.bot.dialogs.prompts.OAuthPrompt;
import com.microsoft.bot.dialogs.prompts.OAuthPromptSettings;
import com.microsoft.bot.dialogs.prompts.PromptOptions;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.schema.TokenResponse;

class MainDialog extends LogoutDialog {

    public MainDialog(Configuration configuration) {
        super("MainDialog", configuration.getProperty("ConnectionName"));

        OAuthPromptSettings settings = new OAuthPromptSettings();
        settings.setText("Please Sign In");
        settings.setTitle("Sign In");
        settings.setConnectionName(configuration.getProperty("ConnectionName"));
        settings.setTimeout(300000); // User has 5 minutes to login (1000 * 60 * 5)

        addDialog(new OAuthPrompt("OAuthPrompt", settings));

        addDialog(new ConfirmPrompt("ConfirmPrompt"));

        WaterfallStep[] waterfallSteps = {
            this::promptStep,
            this::loginStep,
            this::displayTokenPhase1,
            this::displayTokenPhase2
        };

        addDialog(new WaterfallDialog("WaterfallDialog", Arrays.asList(waterfallSteps)));

        // The initial child Dialog to run.
        setInitialDialogId("WaterfallDialog");
    }

    private CompletableFuture<DialogTurnResult> promptStep(WaterfallStepContext stepContext) {
        return stepContext.beginDialog("OAuthPrompt", null);
    }

    private CompletableFuture<DialogTurnResult> loginStep(WaterfallStepContext stepContext) {
        // Get the token from the previous step. Note that we could also have gotten the
        // token directly from the prompt itself. There instanceof an example of this in the next method.
        TokenResponse tokenResponse = (TokenResponse) stepContext.getResult();
        if (tokenResponse != null) {
            return stepContext.getContext().sendActivity(MessageFactory.text("You are now logged in."))
            .thenCompose(result->{
                PromptOptions options = new PromptOptions();
                options.setPrompt(MessageFactory.text("Would you like to view your token?"));
                return stepContext.prompt("ConfirmPrompt", options);
            });
        }

        stepContext.getContext()
            .sendActivity(MessageFactory.text("Login was not successful please try again."));
        return stepContext.endDialog();
    }

    private CompletableFuture<DialogTurnResult> displayTokenPhase1(
        WaterfallStepContext stepContext
    ) {
        stepContext.getContext().sendActivity(MessageFactory.text("Thank you."));

        boolean result = (boolean) stepContext.getResult();
        if (result) {
            // Call the prompt again because we need the token. The reasons for this are:
            // 1. If the user instanceof already logged in we do not need to store the token locally in the bot and worry
            // about refreshing it. We can always just call the prompt again to get the token.
            // 2. We never know how long it will take a user to respond. By the time the
            // user responds the token may have expired. The user would then be prompted to login again.
            //
            // There instanceof no reason to store the token locally in the bot because we can always just call
            // the OAuth prompt to get the token or get a new token if needed.
            return stepContext.beginDialog("OAuthPrompt");
        }

        return stepContext.endDialog();
    }

    private CompletableFuture<DialogTurnResult> displayTokenPhase2(
        WaterfallStepContext stepContext
    ) {
        TokenResponse tokenResponse = (TokenResponse) stepContext.getResult();
        if (tokenResponse != null) {
            stepContext.getContext().sendActivity(MessageFactory.text(
                String.format("Here is your token %s", tokenResponse.getToken()
                )));
        }

        return stepContext.endDialog();
    }
}

