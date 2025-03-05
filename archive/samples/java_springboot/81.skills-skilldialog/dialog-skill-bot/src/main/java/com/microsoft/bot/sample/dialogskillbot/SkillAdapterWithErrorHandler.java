// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogskillbot;

import java.util.concurrent.CompletableFuture;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.OnTurnErrorHandler;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.connector.Async;
import com.microsoft.bot.connector.authentication.AuthenticationConfiguration;
import com.microsoft.bot.integration.BotFrameworkHttpAdapter;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.EndOfConversationCodes;
import com.microsoft.bot.schema.InputHints;

public class SkillAdapterWithErrorHandler extends BotFrameworkHttpAdapter {

    public SkillAdapterWithErrorHandler(
        Configuration configuration,
        AuthenticationConfiguration authenticationConfiguration
    ) {
        super(configuration, authenticationConfiguration);
        setOnTurnError(new SkillAdapterErrorHandler());
    }

    private class SkillAdapterErrorHandler implements OnTurnErrorHandler {

        @Override
        public CompletableFuture<Void> invoke(TurnContext turnContext, Throwable exception) {
            return sendErrorMessage(turnContext, exception).thenAccept(result -> {
                sendEoCToParent(turnContext, exception);
            });
        }

        private CompletableFuture<Void> sendErrorMessage(TurnContext turnContext, Throwable exception) {
            try {
                // Send a message to the user.
                String errorMessageText = "The skill encountered an error or bug.";
                Activity errorMessage =
                    MessageFactory.text(errorMessageText, errorMessageText, InputHints.IGNORING_INPUT);
                return turnContext.sendActivity(errorMessage).thenAccept(result -> {
                    String secondLineMessageText = "To continue to run this bot, please fix the bot source code.";
                    Activity secondErrorMessage =
                        MessageFactory.text(secondLineMessageText, secondLineMessageText, InputHints.EXPECTING_INPUT);
                    turnContext.sendActivity(secondErrorMessage)
                        .thenApply(
                            sendResult -> {
                                // Send a trace activity, which will be displayed in the Bot Framework Emulator.
                                // Note: we return the entire exception in the value property to help the
                                // developer;
                                // this should not be done in production.
                                return TurnContext.traceActivity(
                                    turnContext,
                                    String.format("OnTurnError Trace %s", exception.toString())
                                );

                            }
                        );
                });
            } catch (Exception ex) {
                return Async.completeExceptionally(ex);
            }
        }

        private CompletableFuture<Void> sendEoCToParent(TurnContext turnContext, Throwable exception) {
            try {
                // Send an EndOfConversation activity to the skill caller with the error to end
                // the conversation,
                // and let the caller decide what to do.
                Activity endOfConversation = Activity.createEndOfConversationActivity();
                endOfConversation.setCode(EndOfConversationCodes.SKILL_ERROR);
                endOfConversation.setText(exception.getMessage());
                return turnContext.sendActivity(endOfConversation).thenApply(result -> null);
            } catch (Exception ex) {
                return Async.completeExceptionally(ex);
            }
        }

    }
}
