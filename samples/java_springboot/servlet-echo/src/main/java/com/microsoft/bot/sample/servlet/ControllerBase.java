// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.servlet;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.microsoft.bot.builder.Bot;
import com.microsoft.bot.connector.authentication.AuthenticationException;
import com.microsoft.bot.integration.BotFrameworkHttpAdapter;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.schema.Activity;

import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import java.io.IOException;
import java.io.InputStream;

/**
 * The super class for a Servlet based Bot controller.
 *
 * <p>
 * Subclasses must implement {@link #getBot()}. Other default Bot dependencies
 * are created by {@link ServletWithBotConfiguration}
 * </p>
 */
public abstract class ControllerBase extends ServletWithBotConfiguration {
    private ObjectMapper objectMapper;
    private BotFrameworkHttpAdapter adapter;
    private Bot bot;

    /**
     * Servlet {@link HttpServlet#init()}.
     */
    @Override
    public void init() {
        objectMapper = new ObjectMapper()
            .configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false)
            .findAndRegisterModules();
        createControllerDependencies();
    }

    /**
     * This class needs a {@link BotFrameworkHttpAdapter} and {@link Bot}.
     * Subclasses could override to provide different creation behavior.
     */
    protected void createControllerDependencies() {
        Configuration configuration = getConfiguration();
        adapter = getBotFrameworkHttpAdaptor(configuration);
        bot = getBot();
    }

    /**
     * Receives the incoming Channel message.
     *
     * @param request  The incoming http request.
     * @param response The http response.
     */
    @Override
    protected void doPost(HttpServletRequest request, HttpServletResponse response) {
        try {
            Activity activity = getActivity(request);
            String authHeader = request.getHeader("Authorization");

            adapter.processIncomingActivity(
                authHeader, activity, turnContext -> bot.onTurn(turnContext)
            ).handle((result, exception) -> {
                if (exception == null) {
                    response.setStatus(HttpServletResponse.SC_ACCEPTED);
                    return null;
                }

                if (exception.getCause() instanceof AuthenticationException) {
                    response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
                } else {
                    response.setStatus(HttpServletResponse.SC_INTERNAL_SERVER_ERROR);
                }

                return null;
            });
        } catch (Exception ex) {
            response.setStatus(HttpServletResponse.SC_INTERNAL_SERVER_ERROR);
        }
    }

    // Creates an Activity object from the request
    private Activity getActivity(HttpServletRequest request) throws IOException {
        try (InputStream is = request.getInputStream()) {
            return objectMapper.readValue(is, Activity.class);
        }
    }
}
