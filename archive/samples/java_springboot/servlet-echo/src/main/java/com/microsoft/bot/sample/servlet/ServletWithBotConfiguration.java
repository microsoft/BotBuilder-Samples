// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.servlet;

import com.microsoft.bot.builder.*;
import com.microsoft.bot.connector.authentication.ChannelProvider;
import com.microsoft.bot.connector.authentication.CredentialProvider;
import com.microsoft.bot.integration.*;

import javax.servlet.http.HttpServlet;

/**
 * Provides default factory methods to create Bot dependencies.
 *
 * <p>
 * Subclasses must implement the {@link #getBot()} method to return a Bot
 * object.
 * </p>
 */
public abstract class ServletWithBotConfiguration extends HttpServlet {
    private Storage storage;
    private ConversationState conversationState;
    private UserState userState;
    private Configuration configuration;
    private CredentialProvider credentialProvider;
    private ChannelProvider channelProvider;
    private BotFrameworkHttpAdapter botFrameworkHttpAdapter;

    /**
     * Returns the Configuration for the application.
     *
     * By default, it uses the {@link ClasspathPropertiesConfiguration} class.
     *
     * Default scope of Singleton.
     *
     * @return A Configuration object.
     */
    protected Configuration getConfiguration() {
        if (configuration == null) {
            configuration = new ClasspathPropertiesConfiguration();
        }
        return configuration;
    }

    /**
     * Returns the CredentialProvider for the application.
     *
     * By default, it uses the {@link ConfigurationCredentialProvider} class.
     *
     * Default scope of Singleton.
     *
     * @return A CredentialProvider object.
     */
    protected CredentialProvider getCredentialProvider(Configuration configuration) {
        if (credentialProvider == null) {
            credentialProvider = new ConfigurationCredentialProvider(configuration);
        }
        return credentialProvider;
    }

    /**
     * Returns the ChannelProvider for the application.
     *
     * By default, it uses the {@link ConfigurationChannelProvider} class.
     *
     * Default scope of Singleton.
     *
     * @return A ChannelProvider object.
     */
    protected ChannelProvider getChannelProvider(Configuration configuration) {
        if (channelProvider == null) {
            channelProvider = new ConfigurationChannelProvider(configuration);
        }
        return channelProvider;
    }

    /**
     * Returns the BotFrameworkHttpAdapter for the application.
     *
     * By default, it uses the {@link BotFrameworkHttpAdapter} class.
     *
     * Default scope of Singleton.
     *
     * @return A BotFrameworkHttpAdapter object.
     */
    protected BotFrameworkHttpAdapter getBotFrameworkHttpAdaptor(Configuration configuration) {
        if (botFrameworkHttpAdapter == null) {
            botFrameworkHttpAdapter = new BotFrameworkHttpAdapter(configuration);
        }
        return botFrameworkHttpAdapter;
    }

    /**
     * Returns a {@link Storage} object. Default scope of Singleton.
     *
     * @return A Storage object.
     */
    protected Storage getStorage() {
        if (storage == null) {
            storage = new MemoryStorage();
        }
        return storage;
    }

    /**
     * Returns a ConversationState object.
     *
     * Default scope of Singleton.
     *
     * @param storage The Storage object to use.
     * @return A ConversationState object.
     */
    protected ConversationState getConversationState(Storage storage) {
        if (conversationState == null) {
            conversationState = new ConversationState(storage);
        }
        return conversationState;
    }

    /**
     * Returns a UserState object.
     *
     * Default scope of Singleton.
     *
     * @param storage The Storage object to use.
     * @return A UserState object.
     */
    protected UserState getUserState(Storage storage) {
        if (userState == null) {
            userState = new UserState(storage);
        }
        return userState;
    }

    protected abstract Bot getBot();
}
