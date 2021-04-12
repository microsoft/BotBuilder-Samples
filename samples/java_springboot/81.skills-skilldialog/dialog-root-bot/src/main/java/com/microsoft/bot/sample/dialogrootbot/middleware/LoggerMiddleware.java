// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.
package com.microsoft.bot.sample.dialogrootbot.middleware;

import java.util.concurrent.CompletableFuture;

import com.microsoft.bot.builder.Middleware;
import com.microsoft.bot.builder.NextDelegate;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ActivityTypes;

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.


/**
 * Uses an Logger instance to log user and bot messages. It filters out
 * ContinueConversation events coming from skill responses.
 */
public class LoggerMiddleware implements Middleware {

    private final Logger _logger;

    public LoggerMiddleware(Logger logger) {
        if (logger == null) {
            throw new IllegalArgumentException("Logger cannot be null");
        }
        _logger = logger;
    }

    public CompletableFuture<Void> onTurn(TurnContext turnContext, NextDelegate next) {
        // Note: skill responses will show as ContinueConversation events; we don't log those.
        // We only log incoming messages from users.
        if (!turnContext.getActivity().getType().equals(ActivityTypes.EVENT)
            && turnContext.getActivity().getName() != null
            && turnContext.getActivity().getName().equals("ContinueConversation")) {
            String message = String.format("User said: %s Type: \"%s\" Name: \"%s\"",
                                           turnContext.getActivity().getText(),
                                           turnContext.getActivity().getType(),
                                           turnContext.getActivity().getName());
            _logger.logEntry(message);
        }

        // Register outgoing handler.
        // hook up onSend pipeline
        turnContext.onSendActivities(
            (ctx, activities, nextSend) -> {
                // run full pipeline
                return nextSend.get().thenApply(responses -> {
                    for (Activity activity : activities) {
                        String message = String.format("Bot said: %s Type: \"%s\" Name: \"%s\"",
                                                       activity.getText(),
                                                       activity.getType(),
                                                       activity.getName());
                        _logger.logEntry(message);
                    }
                    return responses;
                });
            }
        );

        if (next != null) {
            return next.next();
        }

        return CompletableFuture.completedFuture(null);
    }
}

