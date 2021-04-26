// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.proactive;

import com.microsoft.bot.integration.BotFrameworkHttpAdapter;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.schema.ConversationReference;
import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

/**
 * This controller will receive GET requests at /api/notify and send a message
 * to all ConversationReferences.
 *
 * @see ConversationReferences
 * @see ProactiveBot
 * @see Application
 */
@RestController
public class NotifyController {
    /**
     * The BotFrameworkHttpAdapter to use. Note is is provided by dependency
     * injection via the constructor.
     *
     * @see com.microsoft.bot.integration.spring.BotDependencyConfiguration
     */
    private final BotFrameworkHttpAdapter adapter;

    private ConversationReferences conversationReferences;
    private String appId;

    @Autowired
    public NotifyController(
        BotFrameworkHttpAdapter withAdapter,
        Configuration withConfiguration,
        ConversationReferences withReferences
    ) {
        adapter = withAdapter;
        conversationReferences = withReferences;
        appId = withConfiguration.getProperty("MicrosoftAppId");
    }

    @GetMapping("/api/notify")
    public ResponseEntity<Object> proactiveMessage() {
        for (ConversationReference reference : conversationReferences.values()) {
            adapter.continueConversation(
                appId, reference, turnContext -> turnContext.sendActivity("proactive hello").thenApply(resourceResponse -> null)
            );
        }

        // Let the caller know proactive messages have been sent
        return new ResponseEntity<>(
            "<html><body><h1>Proactive messages have been sent.</h1></body></html>",
            HttpStatus.ACCEPTED
        );
    }
}
