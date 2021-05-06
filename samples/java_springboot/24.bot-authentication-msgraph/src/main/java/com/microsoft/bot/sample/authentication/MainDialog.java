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
import com.microsoft.bot.dialogs.prompts.ChoicePrompt;
import com.microsoft.bot.dialogs.prompts.OAuthPrompt;
import com.microsoft.bot.dialogs.prompts.OAuthPromptSettings;
import com.microsoft.bot.dialogs.prompts.PromptOptions;
import com.microsoft.bot.dialogs.prompts.TextPrompt;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.schema.TokenResponse;

import org.springframework.stereotype.Component;

@Component
class MainDialog extends LogoutDialog {

    public MainDialog(Configuration configuration) {
        super("MainDialog", configuration.getProperty("ConnectionName"));

        OAuthPromptSettings settings = new OAuthPromptSettings();
        settings.setText("Please login");
        settings.setTitle("Login");
        settings.setConnectionName(configuration.getProperty("ConnectionName"));
        settings.setTimeout(300000); // User has 5 minutes to login (1000 * 60 * 5)
        addDialog(new OAuthPrompt("OAuthPrompt", settings));

        addDialog(new TextPrompt("TextPrompt"));
        addDialog(new ChoicePrompt("ChoicePrompt"));

        WaterfallStep[] waterfallSteps = {
            this::promptStep,
            this::loginStep,
            this::commandStep,
            this::processStep
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
            stepContext.getContext().sendActivity(MessageFactory.text("You are now logged in."))
                    .thenApply(result -> null);
            PromptOptions options = new PromptOptions();
            options.setPrompt(MessageFactory.text("Would you like to do? (type 'me', or 'email')"));
            return stepContext.prompt("TextPrompt", options);
        }

        stepContext.getContext().sendActivity(MessageFactory.text("Login was not successful please try again."))
                .thenApply(result -> null);
        return stepContext.endDialog();
    }

    private CompletableFuture<DialogTurnResult> commandStep(
        WaterfallStepContext stepContext
    ) {

        stepContext.getValues().put("command", stepContext.getResult());

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

    private CompletableFuture<DialogTurnResult> processStep(WaterfallStepContext stepContext) {
        if (stepContext.getResult() != null) {
            // We do not need to store the token in the bot. When we need the token we can
            // send another prompt. If the token is valid the user will not need to log back in.
            // The token will be available in the Result property of the task.
            TokenResponse tokenResponse = null;
            if (stepContext.getResult() instanceof TokenResponse) {
                tokenResponse = (TokenResponse) stepContext.getResult();
            }

            // If we have the token use the user is authenticated so we may use it to make API calls.
            if (tokenResponse != null && tokenResponse.getToken() != null) {

                String command = "";
                if (stepContext.getValues() != null
                    && stepContext.getValues().get("command") != null
                    && stepContext.getValues().get("command") instanceof String) {
                    command = ((String) stepContext.getValues().get("command")).toLowerCase();
                };

                if (command.equals("me")) {
                    OAuthHelpers.ListMeAsync(stepContext.getContext(), tokenResponse)
                            .thenApply(result -> null);
                } else if (command.startsWith("email")) {
                    OAuthHelpers.ListEmailAddressAsync(stepContext.getContext(), tokenResponse)
                            .thenApply(result -> null);
                } else {
                    stepContext.getContext().sendActivity(
                        MessageFactory.text(String.format("Your token is: %s", tokenResponse.getToken())))
                            .thenApply(result -> null);
                }
            }
        } else {
            stepContext.getContext().sendActivity(
                    MessageFactory.text("We couldn't log you in. Please try again later."))
                    .thenApply(result -> null);
        }

        return stepContext.endDialog();
    }
}

