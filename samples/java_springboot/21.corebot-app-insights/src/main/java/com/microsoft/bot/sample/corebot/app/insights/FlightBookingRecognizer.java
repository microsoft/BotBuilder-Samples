// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.corebot.app.insights;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import org.apache.commons.lang3.StringUtils;
import com.microsoft.bot.ai.luis.LuisApplication;
import com.microsoft.bot.ai.luis.LuisRecognizer;
import com.microsoft.bot.ai.luis.LuisRecognizerOptionsV3;
import com.microsoft.bot.builder.BotTelemetryClient;
import com.microsoft.bot.builder.Recognizer;
import com.microsoft.bot.builder.RecognizerResult;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.integration.Configuration;

import java.util.concurrent.CompletableFuture;

/**
 * The class in charge of recognizing the booking information.
 */
public class FlightBookingRecognizer implements Recognizer {
    private final LuisRecognizer recognizer;

    /**
     * The constructor of the FlightBookingRecognizer class.
     *
     * @param configuration The Configuration object to use.
     * @param telemetryClient The BotTelemetryClient to use.
     */
    public FlightBookingRecognizer(Configuration configuration, BotTelemetryClient telemetryClient) {
        LuisRecognizer recognizer1;
        recognizer1 = null;
        Boolean luisIsConfigured = StringUtils.isNotBlank(configuration.getProperty("LuisAppId"))
            && StringUtils.isNotBlank(configuration.getProperty("LuisAPIKey"))
            && StringUtils.isNotBlank(configuration.getProperty("LuisAPIHostName"));

        if (luisIsConfigured) {
            LuisApplication luisApplication = new LuisApplication(
                configuration.getProperty("LuisAppId"),
                configuration.getProperty("LuisAPIKey"),
                "https://".concat(configuration.getProperty("LuisAPIHostName"))
            );

            // Set the recognizer options depending on which endpoint version you want to use.
            // More details can be found in
            // https://docs.microsoft.com/en-gb/azure/cognitive-services/luis/luis-migration-api-v3
            LuisRecognizerOptionsV3 recognizerOptions = new LuisRecognizerOptionsV3(luisApplication);
            recognizerOptions.setTelemetryClient(telemetryClient);

            recognizer1 = new LuisRecognizer(recognizerOptions);
        }
        this.recognizer = recognizer1;
    }

    /**
     * Verify if the recognizer is configured.
     *
     * @return True if it's configured, False if it's not.
     */
    public Boolean isConfigured() {
        return this.recognizer != null;
    }

    /**
     * Return an object with preformatted LUIS results for the bot's dialogs to consume.
     *
     * @param context A {link TurnContext}
     * @return A {link RecognizerResult}
     */
    public CompletableFuture<RecognizerResult> executeLuisQuery(TurnContext context) {
        // Returns true if luis is configured in the application.properties and initialized.
        return this.recognizer.recognize(context);
    }

    /**
     * Gets the From data from the entities which is part of the result.
     *
     * @param result The recognizer result.
     * @return The object node representing the From data.
     */
    public ObjectNode getFromEntities(RecognizerResult result) {
        String fromValue = "", fromAirportValue = "";
        if (result.getEntities().get("$instance").get("From") != null) {
            fromValue = result.getEntities().get("$instance").get("From").get(0).get("text")
                .asText();
        }
        if (!fromValue.isEmpty()
            && result.getEntities().get("From").get(0).get("Airport") != null) {
            fromAirportValue = result.getEntities().get("From").get(0).get("Airport").get(0).get(0)
                .asText();
        }

        ObjectMapper mapper = new ObjectMapper().findAndRegisterModules();
        ObjectNode entitiesNode = mapper.createObjectNode();
        entitiesNode.put("from", fromValue);
        entitiesNode.put("airport", fromAirportValue);
        return entitiesNode;
    }

    /**
     * Gets the To data from the entities which is part of the result.
     *
     * @param result The recognizer result.
     * @return The object node representing the To data.
     */
    public ObjectNode getToEntities(RecognizerResult result) {
        String toValue = "", toAirportValue = "";
        if (result.getEntities().get("$instance").get("To") != null) {
            toValue = result.getEntities().get("$instance").get("To").get(0).get("text").asText();
        }
        if (!toValue.isEmpty() && result.getEntities().get("To").get(0).get("Airport") != null) {
            toAirportValue = result.getEntities().get("To").get(0).get("Airport").get(0).get(0)
                .asText();
        }

        ObjectMapper mapper = new ObjectMapper().findAndRegisterModules();
        ObjectNode entitiesNode = mapper.createObjectNode();
        entitiesNode.put("to", toValue);
        entitiesNode.put("airport", toAirportValue);
        return entitiesNode;
    }

    /**
     * This value will be a TIMEX. And we are only interested in a Date so grab the first result and
     * drop the Time part. TIMEX is a format that represents DateTime expressions that include some
     * ambiguity. e.g. missing a Year.
     *
     * @param result A {link RecognizerResult}
     * @return The Timex value without the Time model
     */
    public String getTravelDate(RecognizerResult result) {
        JsonNode datetimeEntity = result.getEntities().get("datetime");
        if (datetimeEntity == null || datetimeEntity.get(0) == null) {
            return null;
        }

        JsonNode timex = datetimeEntity.get(0).get("timex");
        if (timex == null || timex.get(0) == null) {
            return null;
        }

        String datetime = timex.get(0).asText().split("T")[0];
        return datetime;
    }

    /**
     * Runs an utterance through a recognizer and returns a generic recognizer result.
     *
     * @param turnContext Turn context.
     * @return Analysis of utterance.
     */
    @Override
    public CompletableFuture<RecognizerResult> recognize(TurnContext turnContext) {
        return this.recognizer.recognize(turnContext);
    }
}
