package com.microsoft.bot.sample.authentication;

import java.util.concurrent.CompletableFuture;

import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.schema.TokenResponse;
import com.microsoft.graph.models.extensions.User;

public class OAuthHelpers {
    // Send the user their Graph Display Name from the bot.
    public static CompletableFuture<Void> ListMeAsync(TurnContext turnContext,
                                                      TokenResponse tokenResponse) {
        User user = getUser(turnContext, tokenResponse);
        return turnContext.sendActivity(String.format("You are %s.", user.displayName)).thenApply(result -> null);
    }

    // Send the user their Graph Email Address from the bot.
    public static CompletableFuture<Void> ListEmailAddressAsync(TurnContext turnContext,
                                                                TokenResponse tokenResponse) {
        User user = getUser(turnContext, tokenResponse);
        return turnContext.sendActivity(String.format("Your email: %s.", user.mail)).thenApply(result -> null);
    }

    private static User getUser(TurnContext turnContext, TokenResponse tokenResponse) {
        if (turnContext == null) {
            throw new IllegalArgumentException("turnContext cannot be null");
        }

        if (tokenResponse == null) {
            throw new IllegalArgumentException("tokenResponse cannot be null");
        }

        // Pull in the data from the Microsoft Graph.
        SimpleGraphClient client = new SimpleGraphClient(tokenResponse.getToken());
        return client.getMe();
    }
}
