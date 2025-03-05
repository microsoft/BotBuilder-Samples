// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.inspection;

import com.microsoft.bot.builder.Bot;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.Storage;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.builder.inspection.InspectionState;
import com.microsoft.bot.integration.BotFrameworkHttpAdapter;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.integration.AdapterWithInspection;
import com.microsoft.bot.integration.spring.BotController;
import com.microsoft.bot.integration.spring.BotDependencyConfiguration;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Import;
import org.springframework.context.annotation.Primary;

//
// This is the starting point of the Sprint Boot Bot application.
//
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
 *
 * <p>
 * See README.md for details on using the InspectionMiddleware.
 * </p>
 */
public class Application extends BotDependencyConfiguration {
    public static void main(String[] args) {
        SpringApplication.run(Application.class, args);
    }

    /**
     * Returns the Bot for this application.
     *
     * <p>
     *     The @Component annotation could be used on the Bot class instead of this method
     *     with the @Bean annotation.
     * </p>
     *
     * @return The Bot implementation for this application.
     */
    @Bean
    public Bot getBot(
        ConversationState conversationState,
        UserState userState
    ) {
        return new EchoBot(conversationState, userState);
    }

    /**
     * Create an adapter with InspectionMiddleware.
     *
     * <p>
     * NOTE: This is marked as @Primary to override the default Bean.
     * </p>
     *
     * @param configuration     The configuration.
     *                          {@link BotDependencyConfiguration#getConfiguration()}
     * @param inspectionState   The InspectionState.
     *                          {@link BotDependencyConfiguration#getInspectionState(Storage)}
     * @param userState         The UserState.
     *                          {@link BotDependencyConfiguration#getUserState(Storage)}
     * @param conversationState The ConversationState.
     *                          {@link BotDependencyConfiguration#getConversationState(Storage)}
     * @return An AdapterWithInspection object.
     */
    @Bean
    @Primary
    public BotFrameworkHttpAdapter getInspectionBotFrameworkHttpAdapter(
        Configuration configuration,
        InspectionState inspectionState,
        UserState userState,
        ConversationState conversationState
    ) {
        return new AdapterWithInspection(
            configuration,
            inspectionState,
            userState,
            conversationState
        );
    }
}
