// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.corebot.app.insights;

import com.microsoft.bot.applicationinsights.core.TelemetryInitializerMiddleware;
import com.microsoft.bot.builder.BotTelemetryClient;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.connector.Channels;
import com.microsoft.bot.integration.BotFrameworkHttpAdapter;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ActivityTypes;
import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.exception.ExceptionUtils;
import org.slf4j.LoggerFactory;

import javax.annotation.Nullable;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.CompletableFuture;

/**
 * An Adapter that provides exception handling.
 */
public class AdapterWithErrorHandler extends BotFrameworkHttpAdapter {

    private static final String ERROR_MSG_ONE = "The bot encountered an error or bug.";
    private static final String ERROR_MSG_TWO =
        "To continue to run this bot, please fix the bot source code.";
    // Create field for telemetry client. Add IBotTelemetryClient parameter to AdapterWithErrorHandler
    private BotTelemetryClient adapterBotTelemetryClient;

    /**
     * Constructs an error handling BotFrameworkHttpAdapter by providing an
     * {@link com.microsoft.bot.builder.OnTurnErrorHandler}.
     *
     * <p>
     * For this sample, a simple message is displayed. For a production Bot, a more
     * informative message or action is likely preferred.
     * </p>
     *
     * @param withConfiguration The Configuration object to use.
     * @param telemetryInitializerMiddleware The TelemetryInitializerMiddleware object to use.
     * @param botTelemetryClient The BotTelemetryClient object to use.
     * @param withConversationState The ConversationState object to use.
     */
    public AdapterWithErrorHandler(
        Configuration withConfiguration,
        TelemetryInitializerMiddleware telemetryInitializerMiddleware,
        BotTelemetryClient botTelemetryClient,
        @Nullable ConversationState withConversationState) {
        super(withConfiguration);
        this.use(telemetryInitializerMiddleware);

        // Use telemetry client so that we can trace exceptions into Application Insights
        this.adapterBotTelemetryClient = botTelemetryClient;
        setOnTurnError((turnContext, exception) -> {
            // Track exceptions into Application Insights
            // Set up some properties for our exception tracing to give more information
            Map<String, String> properties = new HashMap<String, String>();
            properties.put("Bot exception caught in", "AdapterWithErrorHandler - OnTurnError");

            // Send the exception telemetry:
            adapterBotTelemetryClient.trackException((Exception) exception, properties, null);

            // Log any leaked exception from the application.
            // NOTE: In production environment, you should consider logging this to
            // Azure Application Insights. Visit https://aka.ms/bottelemetry to see how
            // to add telemetry capture to your bot.
            LoggerFactory.getLogger(AdapterWithErrorHandler.class).error("onTurnError", exception);

            // Send a message to the user
            return turnContext.sendActivities(
                MessageFactory.text(ERROR_MSG_ONE), MessageFactory.text(ERROR_MSG_TWO)
            ).thenCompose(resourceResponse -> sendTraceActivity(turnContext, exception))
                .thenCompose(stageResult -> {
                    if (withConversationState != null) {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a
                        // Web pages.
                        return withConversationState.delete(turnContext)
                            .exceptionally(deleteException -> {
                                LoggerFactory.getLogger(AdapterWithErrorHandler.class)
                                    .error("ConversationState.delete", deleteException);
                                return null;
                            });
                    }
                    return CompletableFuture.completedFuture(null);
                });
        });
    }

    private CompletableFuture<Void> sendTraceActivity(
        TurnContext turnContext,
        Throwable exception
    ) {
        if (StringUtils.equals(turnContext.getActivity().getChannelId(), Channels.EMULATOR)) {
            Activity traceActivity = new Activity(ActivityTypes.TRACE);
            traceActivity.setLabel("TurnError");
            traceActivity.setName("OnTurnError Trace");
            traceActivity.setValue(ExceptionUtils.getStackTrace(exception));
            traceActivity.setValueType("https://www.botframework.com/schemas/error");

            // Send a trace activity, which will be displayed in the Bot Framework Emulator
            return turnContext.sendActivity(traceActivity).thenApply(resourceResponse -> null);
        }

        return CompletableFuture.completedFuture(null);
    }
}
