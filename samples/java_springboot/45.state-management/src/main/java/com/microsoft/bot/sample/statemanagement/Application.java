// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.statemanagement;

import com.microsoft.bot.azure.CosmosDbPartitionedStorage;
import com.microsoft.bot.azure.CosmosDbPartitionedStorageOptions;
import com.microsoft.bot.azure.blobs.BlobsStorage;
import com.microsoft.bot.builder.*;
import com.microsoft.bot.integration.AdapterWithErrorHandler;
import com.microsoft.bot.integration.BotFrameworkHttpAdapter;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.integration.spring.BotController;
import com.microsoft.bot.integration.spring.BotDependencyConfiguration;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Import;

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
        return new StateManagementBot(conversationState, userState);
    }

    /**
     * Returns a custom Adapter that provides error handling.
     *
     * @param configuration The Configuration object to use.
     * @return An error handling BotFrameworkHttpAdapter.
     */
    @Override
    public BotFrameworkHttpAdapter getBotFrameworkHttpAdaptor(Configuration configuration) {
        return new AdapterWithErrorHandler(configuration);
    }

    /**
     * AZURE BLOB STORAGE - Uncomment the code in this section to use Azure blob storage
     * @return A BlobStorage
     */
    /*@Override
    public Storage getStorage() {
        // If using Blob Storage. Fill these connection details in from configuration.
        return new BlobsStorage("<blob-storage-connection-string>", "bot-state");
    }*/

    /**
     * COSMOSDB STORAGE - Uncomment the code in this section to use CosmosDB storage
     * @return A CosmosDbPartitionedStorage
     */
    /*@Override
    public Storage getStorage() {
        // If using CosmosDbPartitionedStorage. Fill these connection details in from configuration.
        CosmosDbPartitionedStorageOptions options = new CosmosDbPartitionedStorageOptions();
        options.setCosmosDbEndpoint("<endpoint-for-your-cosmosdb-instance>");
        options.setAuthKey("<your-cosmosdb-auth-key>");
        options.setDatabaseId("<your-database-id>");
        options.setContainerId("<cosmosdb-container-id>");
        return new CosmosDbPartitionedStorage(options);
    }*/
}
