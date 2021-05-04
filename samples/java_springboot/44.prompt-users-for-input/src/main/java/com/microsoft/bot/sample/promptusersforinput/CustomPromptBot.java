// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.promptusersforinput;

import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.BotState;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.StatePropertyAccessor;
import com.microsoft.bot.dialogs.prompts.PromptCultureModels;
import com.microsoft.recognizers.text.ModelResult;
import com.microsoft.recognizers.text.datetime.DateTimeRecognizer;
import com.microsoft.recognizers.text.number.NumberRecognizer;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.UserState;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.time.temporal.ChronoUnit;
import java.util.List;
import java.util.Map;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.atomic.AtomicReference;

import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.tuple.Triple;
import org.springframework.stereotype.Component;

/**
 * This Bot implementation can run any type of Dialog. The use of type
 * parameterization is to allows multiple different bots to be run at different
 * endpoints within the same project. This can be achieved by defining distinct
 * Controller types each with dependency on distinct Bot types.
 */
public class CustomPromptBot extends ActivityHandler {

    private final BotState userState;
    private final BotState conversationState;

    public CustomPromptBot(ConversationState conversationState, UserState userState) {
        this.conversationState = conversationState;
        this.userState = userState;
    }

    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {

        StatePropertyAccessor<ConversationFlow> conversationStateAccessors =
            conversationState.createProperty("ConversationFlow");

        StatePropertyAccessor<UserProfile> userStateAccessors = userState.createProperty("UserProfile");
        return userStateAccessors.get(turnContext, () -> new UserProfile()).thenCompose(profile -> {
            return conversationStateAccessors.get(turnContext, () -> new ConversationFlow()).thenCompose(flow -> {
                return fillOutUserProfile(flow, profile, turnContext);
            });
        })
        .thenCompose(result -> conversationState.saveChanges(turnContext))
        .thenCompose(result -> userState.saveChanges(turnContext));
    }

    private static CompletableFuture<Void> fillOutUserProfile(ConversationFlow flow,
                                                              UserProfile profile,
                                                              TurnContext turnContext) {
        String input = "";
        if (StringUtils.isNotBlank(turnContext.getActivity().getText())) {
            input = turnContext.getActivity().getText().trim();
        }

        switch (flow.getLastQuestionAsked()) {
            case None:
                return turnContext.sendActivity("Let's get started. What is your name?", null, null)
                    .thenRun(() -> {flow.setLastQuestionAsked(ConversationFlow.Question.Name);});
            case Name:
                Triple<Boolean, String, String> nameValidationResult = validateName(input);
                if (nameValidationResult.getLeft()) {
                    profile.name = nameValidationResult.getMiddle();
                    return turnContext.sendActivity(String.format("Hi %s.", profile.name), null, null)
                        .thenCompose(result -> turnContext.sendActivity("How old are you?", null, null))
                        .thenRun(() -> { flow.setLastQuestionAsked(ConversationFlow.Question.Age); });
                } else {
                    if (StringUtils.isNotBlank(nameValidationResult.getRight())) {
                        return turnContext.sendActivity(nameValidationResult.getRight(), null, null)
                               .thenApply(result -> null);
                    } else {
                        return turnContext.sendActivity("I'm sorry, I didn't understand that.", null, null)
                               .thenApply(result -> null);
                    }
                }
            case Age:
                Triple<Boolean, Integer, String> ageValidationResult = ValidateAge(input);
                if (ageValidationResult.getLeft()) {
                    profile.age = ageValidationResult.getMiddle();
                    return turnContext.sendActivity(String.format("I have your age as %d.", profile.age), null, null)
                        .thenCompose(result -> turnContext.sendActivity("When is your flight?", null, null))
                        .thenRun(() -> { flow.setLastQuestionAsked(ConversationFlow.Question.Date); });
                } else {
                    if (StringUtils.isNotBlank(ageValidationResult.getRight())) {
                        return turnContext.sendActivity(ageValidationResult.getRight(), null, null)
                            .thenApply(result -> null);
                    } else {
                        return turnContext.sendActivity("I'm sorry, I didn't understand that.", null, null)
                            .thenApply(result -> null);
                    }
                }

            case Date:
                Triple<Boolean, String, String> dateValidationResult = ValidateDate(input);
                AtomicReference<UserProfile> profileReference = new AtomicReference<UserProfile>(profile);
                if (dateValidationResult.getLeft()) {
                    profile.date = dateValidationResult.getMiddle();
                    return turnContext.sendActivity(
                        String.format("Your cab ride to the airport is scheduled for %s.",
                                      profileReference.get().date))
                    .thenCompose(result -> turnContext.sendActivity(
                        String.format("Thanks for completing the booking %s.", profileReference.get().name)))
                    .thenCompose(result -> turnContext.sendActivity("Type anything to run the bot again."))
                    .thenRun(() -> {
                        flow.setLastQuestionAsked(ConversationFlow.Question.None);
                        profileReference.set(new UserProfile());
                    });
                } else {
                    if (StringUtils.isNotBlank(dateValidationResult.getRight())) {
                        return turnContext.sendActivity(dateValidationResult.getRight(), null, null)
                            .thenApply(result -> null);
                    } else {
                        return turnContext.sendActivity("I'm sorry, I didn't understand that.", null, null)
                            .thenApply(result -> null);                    }
                }
            default:
                return CompletableFuture.completedFuture(null);
        }
    }

    private static Triple<Boolean, String, String> validateName(String input) {
        String name = null;
        String message = null;

        if (StringUtils.isEmpty(input)) {
            message = "Please enter a name that contains at least one character.";
        } else {
            name = input.trim();
        }

        return Triple.of(StringUtils.isBlank(message), name, message);
    }

    private static Triple<Boolean, Integer, String> ValidateAge(String input) {
        int age = 0;
        String message = null;

        // Try to recognize the input as a number. This works for responses such as "twelve" as well as "12".
        try {
            // Attempt to convert the Recognizer result to an integer. This works for "a dozen", "twelve", "12", and so on.
            // The recognizer returns a list of potential recognition results, if any.

            List<ModelResult> results = NumberRecognizer.recognizeNumber(input, PromptCultureModels.ENGLISH_CULTURE);
            for (ModelResult result : results) {
                // The result resolution is a dictionary, where the "value" entry contains the processed String.
                Object value = result.resolution.get("value");
                if (value != null) {
                    age = Integer.parseInt((String) value);
                    if (age >= 18 && age <= 120) {
                        return Triple.of(true, age, "");
                    }
                }
            }

            message = "Please enter an age between 18 and 120.";
        }
        catch (Throwable th) {
            message = "I'm sorry, I could not interpret that as an age. Please enter an age between 18 and 120.";
        }

        return Triple.of(StringUtils.isBlank(message), age, message);
    }

    private static Triple<Boolean, String, String> ValidateDate(String input) {
        String date = null;
        String message = null;

        // Try to recognize the input as a date-time. This works for responses such as "11/14/2018", "9pm", "tomorrow", "Sunday at 5pm", and so on.
        // The recognizer returns a list of potential recognition results, if any.
        try {
            List<ModelResult> results = DateTimeRecognizer.recognizeDateTime(input, PromptCultureModels.ENGLISH_CULTURE);

            // Check whether any of the recognized date-times are appropriate,
            // and if so, return the first appropriate date-time. We're checking for a value at least an hour in the future.
            LocalDateTime earliest = LocalDateTime.now().plus(1, ChronoUnit.HOURS);

            for (ModelResult result : results) {
                // The result resolution is a dictionary, where the "values" entry contains the processed input.
                List<Map<String, Object>> resolutions = (List<Map<String, Object>>) result.resolution.get("values");

                 for (Map<String, Object> resolution : resolutions) {
                    // The processed input contains a "value" entry if it is a date-time value, or "start" and
                    // "end" entries if it is a date-time range.
                    String dateString = (String) resolution.get("value");
                    if (StringUtils.isBlank(dateString)) {
                        dateString = (String) resolution.get("start");
                    }
                    if (StringUtils.isNotBlank(dateString)){
                        DateTimeFormatter f = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
                        LocalDateTime candidate = LocalDateTime.from(f.parse(dateString));
                        if (earliest.isBefore(candidate)) {
                            DateTimeFormatter dateformat = DateTimeFormatter.ofPattern("MM-dd-yyyy");
                            date = candidate.format(dateformat);
                            return Triple.of(true, date, message);
                        }
                    }

                 }
            }

            message = "I'm sorry, please enter a date at least an hour out.";
        }
        catch (Throwable th) {
            message = "I'm sorry, I could not interpret that as an appropriate date. Please enter a date at least an hour out.";
        }

        return Triple.of(false, date, message);
    }
}
