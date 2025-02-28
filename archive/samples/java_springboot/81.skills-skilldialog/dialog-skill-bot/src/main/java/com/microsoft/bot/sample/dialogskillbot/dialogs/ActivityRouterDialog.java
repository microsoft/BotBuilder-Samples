// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogskillbot.dialogs;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CompletableFuture;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.RecognizerResult;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.Dialog;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.DialogTurnStatus;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.restclient.serializer.JacksonAdapter;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ActivityTypes;
import com.microsoft.bot.schema.InputHints;
import com.microsoft.bot.schema.Serialization;

/**
 * A root dialog that can route activities sent to the skill to different
 * sub-dialogs.
 */
public class ActivityRouterDialog extends ComponentDialog {

    private final DialogSkillBotRecognizer luisRecognizer;

    public ActivityRouterDialog(DialogSkillBotRecognizer luisRecognizer) {
        super("ActivityRouterDialog");
        this.luisRecognizer = luisRecognizer;

        addDialog(new BookingDialog());
        List<WaterfallStep> stepList = new ArrayList<WaterfallStep>();
        stepList.add(this::processActivity);
        addDialog(new WaterfallDialog("WaterfallDialog", stepList));

        // The initial child Dialog to run.
        setInitialDialogId("WaterfallDialog");
    }

    private CompletableFuture<DialogTurnResult> processActivity(WaterfallStepContext stepContext) {
        // A skill can send trace activities, if needed.
        TurnContext.traceActivity(
            stepContext.getContext(),
            String.format(
                "{%s}.processActivity() Got ActivityType: %s",
                this.getClass().getName(),
                stepContext.getContext().getActivity().getType()
            )
        );

        switch (stepContext.getContext().getActivity().getType()) {
            case ActivityTypes.EVENT:
                return onEventActivity(stepContext);

            case ActivityTypes.MESSAGE:
                return onMessageActivity(stepContext);

            default:
                String defaultMessage = String
                    .format("Unrecognized ActivityType: \"%s\".", stepContext.getContext().getActivity().getType());
                // We didn't get an activity type we can handle.
                return stepContext.getContext()
                    .sendActivity(MessageFactory.text(defaultMessage, defaultMessage, InputHints.IGNORING_INPUT))
                    .thenCompose(result -> {
                        return CompletableFuture.completedFuture(new DialogTurnResult(DialogTurnStatus.COMPLETE));
                    });

        }
    }

    // This method performs different tasks super. on the event name.
    private CompletableFuture<DialogTurnResult> onEventActivity(WaterfallStepContext stepContext) {
        Activity activity = stepContext.getContext().getActivity();
        TurnContext.traceActivity(
            stepContext.getContext(),
            String.format(
                "%s.onEventActivity(), label: %s, Value: %s",
                this.getClass().getName(),
                activity.getName(),
                GetObjectAsJsonString(activity.getValue())
            )
        );

        // Resolve what to execute super. on the event name.
        switch (activity.getName()) {
            case "BookFlight":
                return beginBookFlight(stepContext);

            case "GetWeather":
                return beginGetWeather(stepContext);

            default:
                String message = String.format("Unrecognized EventName: \"%s\".", activity.getName());
                // We didn't get an event name we can handle.
                stepContext.getContext().sendActivity(MessageFactory.text(message, message, InputHints.IGNORING_INPUT));
                return CompletableFuture.completedFuture(new DialogTurnResult(DialogTurnStatus.COMPLETE));
        }
    }

    // This method just gets a message activity and runs it through LUS.
    private CompletableFuture<DialogTurnResult> onMessageActivity(WaterfallStepContext stepContext) {
        Activity activity = stepContext.getContext().getActivity();
        TurnContext.traceActivity(
            stepContext.getContext(),
            String.format(
                "%s.onMessageActivity(), label: %s, Value: %s",
                this.getClass().getName(),
                activity.getName(),
                GetObjectAsJsonString(activity.getValue())
            )
        );

        if (!luisRecognizer.getIsConfigured()) {
            String message = "NOTE: LUIS instanceof not configured. To enable all capabilities, add 'LuisAppId',"
                + " 'LuisAPKey' and 'LuisAPHostName' to the appsettings.json file.";
            return stepContext.getContext()
                .sendActivity(MessageFactory.text(message, message, InputHints.IGNORING_INPUT))
                .thenCompose(
                    result -> CompletableFuture.completedFuture(new DialogTurnResult(DialogTurnStatus.COMPLETE))
                );
        } else {
            // Call LUIS with the utterance.
            return luisRecognizer.recognize(stepContext.getContext(), RecognizerResult.class)
                .thenCompose(luisResult -> {
                    // Create a message showing the LUS results.
                    StringBuilder sb = new StringBuilder();
                    sb.append(String.format("LUIS results for \"%s\":", activity.getText()));

                    sb.append(
                        String.format(
                            "Intent: \"%s\" Score: %s",
                            luisResult.getTopScoringIntent().intent,
                            luisResult.getTopScoringIntent().score
                        )
                    );

                    return stepContext.getContext()
                        .sendActivity(MessageFactory.text(sb.toString(), sb.toString(), InputHints.IGNORING_INPUT))
                        .thenCompose(result -> {
                            switch (luisResult.getTopScoringIntent().intent.toLowerCase()) {
                                case "bookflight":
                                    return beginBookFlight(stepContext);

                                case "getweather":
                                    return beginGetWeather(stepContext);

                                default:
                                    // Catch all for unhandled intents.
                                    String didntUnderstandMessageText = String.format(
                                        "Sorry, I didn't get that. Please try asking in a different "
                                        + "way (intent was %s)",
                                        luisResult.getTopScoringIntent().intent
                                    );
                                    Activity didntUnderstandMessage = MessageFactory.text(
                                        didntUnderstandMessageText,
                                        didntUnderstandMessageText,
                                        InputHints.IGNORING_INPUT
                                    );
                                    return stepContext.getContext()
                                        .sendActivity(didntUnderstandMessage)
                                        .thenCompose(
                                            stepResult -> CompletableFuture
                                                .completedFuture(new DialogTurnResult(DialogTurnStatus.COMPLETE))
                                        );

                            }
                        });
                    // Start a dialog if we recognize the intent.
                });
        }
    }

    private static CompletableFuture<DialogTurnResult> beginGetWeather(WaterfallStepContext stepContext) {
        Activity activity = stepContext.getContext().getActivity();
        Location location = new Location();
        if (activity.getValue() != null) {
            try {
                location = Serialization.safeGetAs(activity.getValue(), Location.class);
            } catch (JsonProcessingException e) {
                // something went wrong, so we create an empty Location so we won't get a null
                // reference below when we acess location.
                location = new Location();
            }

        }

        // We haven't implemented the GetWeatherDialog so we just display a TODO
        // message.
        String getWeatherMessageText = String
            .format("TODO: get weather for here (lat: %s, long: %s)", location.getLatitude(), location.getLongitude());
        Activity getWeatherMessage =
            MessageFactory.text(getWeatherMessageText, getWeatherMessageText, InputHints.IGNORING_INPUT);
        return stepContext.getContext().sendActivity(getWeatherMessage).thenCompose(result -> {
            return CompletableFuture.completedFuture(new DialogTurnResult(DialogTurnStatus.COMPLETE));
        });

    }

    private CompletableFuture<DialogTurnResult> beginBookFlight(WaterfallStepContext stepContext) {
        Activity activity = stepContext.getContext().getActivity();
        BookingDetails bookingDetails = new BookingDetails();
        if (activity.getValue() != null) {
            try {
                bookingDetails = Serialization.safeGetAs(activity.getValue(), BookingDetails.class);
            } catch (JsonProcessingException e) {
                // we already initialized bookingDetails above, so the flow will run as if
                // no details were sent.
            }
        }

        // Start the booking dialog.
        Dialog bookingDialog = findDialog("BookingDialog");
        return stepContext.beginDialog(bookingDialog.getId(), bookingDetails);
    }

    private String GetObjectAsJsonString(Object value) {
        try {
            return new JacksonAdapter().serialize(value);
        } catch (IOException e) {
            return null;
        }
    }
}
