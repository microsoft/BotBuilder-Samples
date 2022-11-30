// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamsauth;

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

// This class is a wrapper for the Microsoft Graph API
// See: https://developer.microsoft.com/en-us/graph
public class SimpleGraphClient {
    private final String token;
    public SimpleGraphClient(String token) {
        if (StringUtils.isBlank(token)) {
            throw new IllegalArgumentException("token can't be null or empty");
        }
        this.token = token;
    }

    // Get information about the user.
    public User getMe() {
        IGraphServiceClient client = getAuthenticatedClient();
        final List<Option> options = new LinkedList<Option>();
        User user = client.me().buildRequest(options).get();
        return user;
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
