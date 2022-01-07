// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package <%= packageName %>;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.prompts.PromptOptions;
import com.microsoft.bot.dialogs.prompts.TextPrompt;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.InputHints;
import com.microsoft.recognizers.datatypes.timex.expression.TimexProperty;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CompletableFuture;

import com.fasterxml.jackson.databind.node.ObjectNode;
import org.apache.commons.lang3.StringUtils;

/**
 * The class containing the main dialog for the sample.
 */
public class MainDialog extends ComponentDialog {

    private final FlightBookingRecognizer luisRecognizer;
    private final Integer plusDayValue = 7;

    /**
     * The constructor of the Main Dialog class.
     *
     * @param withLuisRecognizer The FlightBookingRecognizer object.
     * @param bookingDialog      The BookingDialog object with booking dialogs.
     */
    public MainDialog(FlightBookingRecognizer withLuisRecognizer, BookingDialog bookingDialog) {
        super("MainDialog");

        luisRecognizer = withLuisRecognizer;

        addDialog(new TextPrompt("TextPrompt"));
        addDialog(bookingDialog);
        WaterfallStep[] waterfallSteps = {
            this::introStep,
            this::actStep,
            this::finalStep
        };
        addDialog(new WaterfallDialog("WaterfallDialog", Arrays.asList(waterfallSteps)));

        // The initial child Dialog to run.
        setInitialDialogId("WaterfallDialog");
    }

    /**
     * First step in the waterfall dialog. Prompts the user for a command. Currently, this expects a
     * booking request, like "book me a flight from Paris to Berlin on march 22" Note that the
     * sample LUIS model will only recognize Paris, Berlin, New York and London as airport cities.
     *
     * @param stepContext A {@link WaterfallStepContext}
     * @return A {@link DialogTurnResult}
     */
    private CompletableFuture<DialogTurnResult> introStep(WaterfallStepContext stepContext) {
        if (!luisRecognizer.isConfigured()) {
            Activity text = MessageFactory.text("NOTE: LUIS is not configured. "
                + "To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' "
                + "to the appsettings.json file.", null, InputHints.IGNORING_INPUT);
            return stepContext.getContext().sendActivity(text)
                .thenCompose(sendResult -> stepContext.next(null));
        }

        // Use the text provided in FinalStepAsync or the default if it is the first time.
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("MMMM d, yyyy");
        String weekLaterDate = LocalDateTime.now().plusDays(plusDayValue).format(formatter);
        String messageText = stepContext.getOptions() != null
            ? stepContext.getOptions().toString()
            : String.format("What can I help you with today?\n"
                + "Say something like \"Book a flight from Paris to Berlin on %s\"", weekLaterDate);
        Activity promptMessage = MessageFactory
            .text(messageText, messageText, InputHints.EXPECTING_INPUT);
        PromptOptions promptOptions = new PromptOptions();
        promptOptions.setPrompt(promptMessage);
        return stepContext.prompt("TextPrompt", promptOptions);
    }

    /**
     * Second step in the waterfall.  This will use LUIS to attempt to extract the origin,
     * destination and travel dates. Then, it hands off to the bookingDialog child dialog to collect
     * any remaining details.
     *
     * @param stepContext A {@link WaterfallStepContext}
     * @return A {@link DialogTurnResult}
     */
    private CompletableFuture<DialogTurnResult> actStep(WaterfallStepContext stepContext) {
        if (!luisRecognizer.isConfigured()) {
            // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
            return stepContext.beginDialog("BookingDialog", new BookingDetails());
        }

        // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
        return luisRecognizer.recognize(stepContext.getContext()).thenCompose(luisResult -> {
            switch (luisResult.getTopScoringIntent().intent) {
                case "BookFlight":
                    // Extract the values for the composite entities from the LUIS result.
                    ObjectNode fromEntities = luisRecognizer.getFromEntities(luisResult);
                    ObjectNode toEntities = luisRecognizer.getToEntities(luisResult);

                    // Show a warning for Origin and Destination if we can't resolve them.
                    return showWarningForUnsupportedCities(
                        stepContext.getContext(), fromEntities, toEntities)
                        .thenCompose(showResult -> {
                                // Initialize BookingDetails with any entities we may have found in the response.

                                BookingDetails bookingDetails = new BookingDetails();
                                bookingDetails.setDestination(toEntities.get("airport").asText());
                                bookingDetails.setOrigin(fromEntities.get("airport").asText());
                                bookingDetails.setTravelDate(luisRecognizer.getTravelDate(luisResult));
                                // Run the BookingDialog giving it whatever details we have from the LUIS call,
                                // it will fill out the remainder.
                                return stepContext.beginDialog("BookingDialog", bookingDetails);
                            }
                        );
                case "GetWeather":
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    String getWeatherMessageText = "TODO: get weather flow here";
                    Activity getWeatherMessage = MessageFactory
                        .text(
                            getWeatherMessageText, getWeatherMessageText,
                            InputHints.IGNORING_INPUT
                        );
                    return stepContext.getContext().sendActivity(getWeatherMessage)
                        .thenCompose(resourceResponse -> stepContext.next(null));

                default:
                    // Catch all for unhandled intents
                    String didntUnderstandMessageText = String.format(
                        "Sorry, I didn't get that. Please "
                            + " try asking in a different way (intent was %s)",
                        luisResult.getTopScoringIntent().intent
                    );
                    Activity didntUnderstandMessage = MessageFactory
                        .text(
                            didntUnderstandMessageText, didntUnderstandMessageText,
                            InputHints.IGNORING_INPUT
                        );
                    return stepContext.getContext().sendActivity(didntUnderstandMessage)
                        .thenCompose(resourceResponse -> stepContext.next(null));
            }
        });
    }

    /**
     * Shows a warning if the requested From or To cities are recognized as entities but they are
     * not in the Airport entity list. In some cases LUIS will recognize the From and To composite
     * entities as a valid cities but the From and To Airport values will be empty if those entity
     * values can't be mapped to a canonical item in the Airport.
     *
     * @param turnContext  A {@link WaterfallStepContext}
     * @param fromEntities An ObjectNode with the entities of From object
     * @param toEntities   An ObjectNode with the entities of To object
     * @return A task
     */
    private static CompletableFuture<Void> showWarningForUnsupportedCities(
        TurnContext turnContext,
        ObjectNode fromEntities,
        ObjectNode toEntities
    ) {
        List<String> unsupportedCities = new ArrayList<String>();

        if (StringUtils.isNotBlank(fromEntities.get("from").asText())
            && StringUtils.isBlank(fromEntities.get("airport").asText())) {
            unsupportedCities.add(fromEntities.get("from").asText());
        }

        if (StringUtils.isNotBlank(toEntities.get("to").asText())
            && StringUtils.isBlank(toEntities.get("airport").asText())) {
            unsupportedCities.add(toEntities.get("to").asText());
        }

        if (!unsupportedCities.isEmpty()) {
            String messageText = String.format(
                "Sorry but the following airports are not supported: %s",
                String.join(", ", unsupportedCities)
            );
            Activity message = MessageFactory
                .text(messageText, messageText, InputHints.IGNORING_INPUT);
            return turnContext.sendActivity(message)
                .thenApply(sendResult -> null);
        }

        return CompletableFuture.completedFuture(null);
    }

    /**
     * This is the final step in the main waterfall dialog. It wraps up the sample "book a flight"
     * interaction with a simple confirmation.
     *
     * @param stepContext A {@link WaterfallStepContext}
     * @return A {@link DialogTurnResult}
     */
    private CompletableFuture<DialogTurnResult> finalStep(WaterfallStepContext stepContext) {
        CompletableFuture<Void> stepResult = CompletableFuture.completedFuture(null);

        // If the child dialog ("BookingDialog") was cancelled,
        // the user failed to confirm or if the intent wasn't BookFlight
        // the Result here will be null.
        if (stepContext.getResult() instanceof BookingDetails) {
            // Now we have all the booking details call the booking service.
            // If the call to the booking service was successful tell the user.
            BookingDetails result = (BookingDetails) stepContext.getResult();
            TimexProperty timeProperty = new TimexProperty(result.getTravelDate());
            String travelDateMsg = timeProperty.toNaturalLanguage(LocalDateTime.now());
            String messageText = String.format("I have you booked to %s from %s on %s",
                result.getDestination(), result.getOrigin(), travelDateMsg
            );
            Activity message = MessageFactory
                .text(messageText, messageText, InputHints.IGNORING_INPUT);
            stepResult = stepContext.getContext().sendActivity(message).thenApply(sendResult -> null);
        }

        // Restart the main dialog with a different message the second time around
        String promptMessage = "What else can I do for you?";
        return stepResult
            .thenCompose(result -> stepContext.replaceDialog(getInitialDialogId(), promptMessage));
    }
}
