// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.corebot.app.insights;

import com.microsoft.bot.applicationinsights.ApplicationInsightsBotTelemetryClient;
import com.microsoft.bot.applicationinsights.core.TelemetryInitializerMiddleware;
import com.microsoft.bot.builder.Bot;
import com.microsoft.bot.builder.BotTelemetryClient;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.NullBotTelemetryClient;
import com.microsoft.bot.builder.Storage;
import com.microsoft.bot.builder.TelemetryLoggerMiddleware;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.integration.BotFrameworkHttpAdapter;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.integration.spring.BotController;
import com.microsoft.bot.integration.spring.BotDependencyConfiguration;
import org.apache.commons.lang3.StringUtils;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Import;

/**
 * This is the starting point of the Sprint Boot Bot application.
 */
@SpringBootApplication

// Use the default BotController to receive incoming Channel messages. A custom
// controller could be used by eliminating this import and creating a new
// org.springframework.web.bind.annotation.RestController.
// The default controller is created by the Spring Boot container using
// dependency injection. The default route is /api/messages.
@Import({BotController.class})

/**
 * This class extends the BotDependencyConfiguration which provides the default
 * implementations for a Bot application.  The Application class should
 * override methods in order to provide custom implementations.
 */
public class Application extends BotDependencyConfiguration {

    /**
     * The start method.
     *
     * @param args The args.
     */
    public static void main(String[] args) {
        SpringApplication.run(Application.class, args);
    }

    /**
     * Returns the Bot for this application.
     *
     * <p>
     * The @Component annotation could be used on the Bot class instead of this method with the
     * @Bean annotation.
     * </p>
     *
     * @param configuration The Configuration object to use.
     * @param userState The UserState object to use.
     * @param conversationState The ConversationState object to use.
     * @return The Bot implementation for this application.
     */
    @Bean
    public Bot getBot(
        Configuration configuration,
        UserState userState,
        ConversationState conversationState
    ) {
        BotTelemetryClient botTelemetryClient = getBotTelemetryClient(configuration);
        FlightBookingRecognizer recognizer = new FlightBookingRecognizer(configuration, botTelemetryClient);
        MainDialog dialog = new MainDialog(recognizer, new BookingDialog(), botTelemetryClient);

        return new DialogAndWelcomeBot<>(conversationState, userState, dialog);
    }

    /**
     * Returns a custom Adapter that provides error handling.
     *
     * @param configuration The Configuration object to use.
     * @return An error handling BotFrameworkHttpAdapter.
     */
    @Override
    public BotFrameworkHttpAdapter getBotFrameworkHttpAdaptor(Configuration configuration) {
        Storage storage = getStorage();
        ConversationState conversationState = getConversationState(storage);
        BotTelemetryClient botTelemetryClient = getBotTelemetryClient(configuration);
        TelemetryLoggerMiddleware telemetryLoggerMiddleware = new TelemetryLoggerMiddleware(botTelemetryClient, false);
        TelemetryInitializerMiddleware telemetryInitializerMiddleware =
            new TelemetryInitializerMiddleware(telemetryLoggerMiddleware, false);

        AdapterWithErrorHandler adapter = new AdapterWithErrorHandler(
            configuration,
            telemetryInitializerMiddleware,
            botTelemetryClient,
            conversationState);

        return adapter;
    }

    /**
     * Returns a Bot Telemetry Client.
     *
     * @param configuration The Configuration object to use.
     * @return A Bot Telemetry Client.
     */
    @Bean
    public BotTelemetryClient getBotTelemetryClient(Configuration configuration) {
        String instrumentationKey = configuration.getProperty("ApplicationInsights.InstrumentationKey");
        if (StringUtils.isNotBlank(instrumentationKey)) {
            return new ApplicationInsightsBotTelemetryClient(instrumentationKey);
        }

        return new NullBotTelemetryClient();
    }
}
