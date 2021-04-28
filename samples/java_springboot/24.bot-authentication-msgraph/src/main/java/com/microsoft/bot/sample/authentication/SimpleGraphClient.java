// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.authentication;

import com.microsoft.graph.logger.DefaultLogger;
import com.microsoft.graph.logger.LoggerLevel;
import com.microsoft.graph.models.extensions.User;
import com.microsoft.graph.options.Option;
import com.microsoft.graph.requests.extensions.GraphServiceClient;
import com.microsoft.graph.models.extensions.IGraphServiceClient;

import java.util.LinkedList;
import java.util.List;

public class SimpleGraphClient {
    private String token;
    public SimpleGraphClient(String token) {
        this.token = token;
    }

    public User getMe() {
        IGraphServiceClient client = getAuthenticatedClient();
        final List<Option> options = new LinkedList<Option>();
        User user = client.me().buildRequest(options).get();
        return user;
    }

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
