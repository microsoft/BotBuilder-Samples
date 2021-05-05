// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.nlpwithdispatch;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.concurrent.CompletableFuture;
import java.util.stream.Collectors;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.fasterxml.jackson.databind.JsonNode;
import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.RecognizerResult;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.RecognizerResult.NamedIntentScore;
import com.microsoft.bot.schema.ChannelAccount;

import org.apache.commons.lang3.StringUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class DispatchBot extends ActivityHandler {

    private final Logger logger;
    private final BotServices botServices;

    public DispatchBot(BotServices botServices) {
        logger = LoggerFactory.getLogger(DispatchBot.class);
        this.botServices = botServices;
    }

    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
        return botServices.getDispatch().recognize(turnContext).thenCompose(recognizerResult -> {
            // Top intent tell us which cognitive service to use.
            NamedIntentScore topIntent = recognizerResult.getTopScoringIntent();

            // Next, we call the dispatcher with the top intent.
            return dispatchToTopIntent(turnContext, topIntent.intent, recognizerResult).thenApply(task -> null);
        });
    }

    @Override
    protected CompletableFuture<Void> onMembersAdded(List<ChannelAccount> membersAdded, TurnContext turnContext) {
        final String WelcomeText = "Type a greeting, or a question about the weather to get started.";

        return membersAdded.stream()
            .filter(member -> !StringUtils.equals(member.getId(), turnContext.getActivity().getRecipient().getId()))
            .map(member -> {
                String msg = String.format("Welcome to Dispatch bot %s. %s", member.getName(), WelcomeText);
                return turnContext.sendActivity(MessageFactory.text(msg));
            })
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponses -> null);
    }

    private CompletableFuture<Void> dispatchToTopIntent(
        TurnContext turnContext,
        String intent,
        RecognizerResult recognizerResult
    ) {
        switch (intent) {
            case "l_HomeAutomation":
                return processHomeAutomation(turnContext, recognizerResult);

            case "l_Weather":
                return processWeather(turnContext, recognizerResult);

            case "q_sample-qna":
                return processSampleQnA(turnContext);

            default:
                logger.info(String.format("Dispatch unrecognized intent: %s.", intent));
                return turnContext
                    .sendActivity(MessageFactory.text(String.format("Dispatch unrecognized intent: %s.", intent)))
                    .thenApply(result -> null);
        }
    }

    private CompletableFuture<Void> processHomeAutomation(TurnContext turnContext, RecognizerResult luisResult) {
        logger.info("ProcessHomeAutomation");

        // Retrieve LUIS result for Process Automation.
        PredictionResult predictionResult = mapPredictionResult(luisResult.getProperties().get("luisResult"));

        Intent topIntent = predictionResult.getIntents().get(0);
        return turnContext
            .sendActivity(MessageFactory.text(String.format("HomeAutomation top intent %s.", topIntent.getTopIntent())))
            .thenCompose(sendResult -> {
                List<String> intents =
                    topIntent.getChildIntents().stream().map(x -> x.getName()).collect(Collectors.toList());
                return turnContext
                    .sendActivity(
                        MessageFactory
                            .text(String.format("HomeAutomation intents detected:\n\n%s", String.join("\n\n", intents)))
                    )
                    .thenCompose(nextSendResult -> {
                        if (luisResult.getEntities() != null) {
                            List<String> entities = mapEntities(luisResult.getEntities());
                            if (entities.size() > 0) {
                                return turnContext
                                    .sendActivity(
                                        MessageFactory.text(
                                            String.format(
                                                "HomeAutomation entities were found in the message:\n\n%s",
                                                String.join("\n\n", entities)
                                            )
                                        )
                                    )
                                    .thenApply(finalSendResult -> null);
                            }
                        }
                        return CompletableFuture.completedFuture(null);
                    });
            });
    }

    private List<String> mapEntities(JsonNode entityNode) {
        List<String> entities = new ArrayList<String>();
        for (Iterator<Map.Entry<String, JsonNode>> child = entityNode.fields(); child.hasNext();) {
            Map.Entry<String, JsonNode> childIntent = child.next();
            String childName = childIntent.getKey();
            if (!childName.startsWith("$")) {
                entities.add(childIntent.getValue().get(0).asText());
            }
        }
        return entities;
    }

    private PredictionResult mapPredictionResult(JsonNode luisResult) {
        JsonNode prediction = luisResult.get("prediction");
        JsonNode intentsObject = prediction.get("intents");
        if (intentsObject == null) {
            return null;
        }
        PredictionResult result = new PredictionResult();
        result.setTopIntent(prediction.get("topIntent").asText());
        List<Intent> intents = new ArrayList<Intent>();
        for (Iterator<Map.Entry<String, JsonNode>> it = intentsObject.fields(); it.hasNext();) {
            Map.Entry<String, JsonNode> intent = it.next();
            double score = intent.getValue().get("score").asDouble();
            String intentName = intent.getKey().replace(".", "_").replace(" ", "_");
            Intent newIntent = new Intent();
            newIntent.setName(intentName);
            newIntent.setScore(score);
            JsonNode childNode = intent.getValue().get("childApp");
            if (childNode != null) {
                newIntent.setTopIntent(childNode.get("topIntent").asText());
                List<Intent> childIntents = new ArrayList<Intent>();
                JsonNode childIntentNodes = childNode.get("intents");
                for (Iterator<Map.Entry<String, JsonNode>> child = childIntentNodes.fields(); child.hasNext();) {
                    Map.Entry<String, JsonNode> childIntent = child.next();
                    double childScore = childIntent.getValue().get("score").asDouble();
                    String childIntentName = childIntent.getKey();
                    Intent newChildIntent = new Intent();
                    newChildIntent.setName(childIntentName);
                    newChildIntent.setScore(childScore);
                    childIntents.add(newChildIntent);
                }
                newIntent.setChildIntents(childIntents);
            }

            intents.add(newIntent);
        }
        result.setIntents(intents);
        return result;
    }

    private CompletableFuture<Void> processWeather(TurnContext turnContext, RecognizerResult luisResult) {
        logger.info("ProcessWeather");

        // Retrieve LUIS result for Weather.
        PredictionResult predictionResult = mapPredictionResult(luisResult.getProperties().get("luisResult"));

        Intent topIntent = predictionResult.getIntents().get(0);
        return turnContext
            .sendActivity(MessageFactory.text(String.format("ProcessWeather top intent %s.", topIntent.getTopIntent())))
            .thenCompose(sendResult -> {
                List<String> intents =
                    topIntent.getChildIntents().stream().map(x -> x.getName()).collect(Collectors.toList());
                return turnContext
                    .sendActivity(
                        MessageFactory
                            .text(String.format("ProcessWeather Intents detected:\n\n%s", String.join("\n\n", intents)))
                    )
                    .thenCompose(secondResult -> {
                        if (luisResult.getEntities() != null) {
                            List<String> entities = mapEntities(luisResult.getEntities());
                            if (entities.size() > 0) {
                                return turnContext
                                    .sendActivity(
                                        MessageFactory.text(
                                            String.format(
                                                "ProcessWeather entities were found in the message:\n\n%s",
                                                String.join("\n\n", entities)
                                            )
                                        )
                                    )
                                    .thenApply(finalResult -> null);
                            }
                        }
                        return CompletableFuture.completedFuture(null);
                    });
            });
    }

    private CompletableFuture<Void> processSampleQnA(TurnContext turnContext) {
        logger.info("ProcessSampleQnA");

        return botServices.getSampleQnA().getAnswers(turnContext, null).thenCompose(results -> {
            if (results.length > 0) {
                return turnContext.sendActivity(MessageFactory.text(results[0].getAnswer())).thenApply(result -> null);
            } else {
                return turnContext
                    .sendActivity(MessageFactory.text("Sorry, could not find an answer in the Q and A system."))
                    .thenApply(result -> null);
            }
        });
    }
}
