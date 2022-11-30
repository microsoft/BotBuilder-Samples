// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamssearchauth;


import com.microsoft.graph.logger.DefaultLogger;
import com.microsoft.graph.logger.LoggerLevel;
import com.microsoft.graph.models.extensions.Message;
import com.microsoft.graph.models.extensions.User;
import com.microsoft.graph.options.Option;
import com.microsoft.graph.options.QueryOption;
import com.microsoft.graph.requests.extensions.GraphServiceClient;
import com.microsoft.graph.models.extensions.IGraphServiceClient;
import com.microsoft.graph.requests.extensions.IMessageCollectionPage;
import org.apache.commons.lang3.StringUtils;

import java.util.LinkedList;
import java.util.List;
import java.util.concurrent.CompletableFuture;

public class SimpleGraphClient {
    private final String token;
    public SimpleGraphClient(String token) {
        if (StringUtils.isBlank(token)) {
            throw new IllegalArgumentException("token cannot be null");
        }
        this.token = token;
    }

    // Searches the user's mail Inbox using the Microsoft Graph API
    public List<Message> searchMailInbox(String search) {
        IGraphServiceClient client = getAuthenticatedClient();

        final List<Option> options = new LinkedList<Option>();
        options.add(new QueryOption("search", search));

        IMessageCollectionPage message = client.me().messages()
            .buildRequest(options)
            .get();

        return message.getCurrentPage().subList(0, 10);
    }

    // Fetching user's profile
    public User getMyProfile() {
        IGraphServiceClient graphClient = this.getAuthenticatedClient();
        return graphClient.me().buildRequest().get();
    }

    // Get an Authenticated Microsoft Graph client using the token issued to the user.
    private IGraphServiceClient getAuthenticatedClient() {
        // Create default logger to only log errors
        DefaultLogger logger = new DefaultLogger();
        logger.setLoggingLevel(LoggerLevel.ERROR);

        // Build a Graph client
        return GraphServiceClient.builder()
            .authenticationProvider(request -> {
                // Add the access token in the Authorization header
                request.addHeader("Authorization", "Bearer " + SimpleGraphClient.this.token);
            })
            .logger(logger)
            .buildClient();
    }
}
