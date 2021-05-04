// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.customdialogs;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.StatePropertyAccessor;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.prompts.NumberPrompt;
import com.microsoft.bot.dialogs.prompts.PromptValidatorContext;
import com.microsoft.bot.dialogs.prompts.TextPrompt;
import com.microsoft.recognizers.text.Culture;

import java.util.*;
import java.util.concurrent.CompletableFuture;

// This is an example root dialog. Replace this with your applications.
public class RootDialog extends ComponentDialog {

    private StatePropertyAccessor<Map<String, Map<String, String>>> userStateAccessor;

    public RootDialog(UserState withUserState) {
        super("root");

        userStateAccessor = withUserState.createProperty("result");

        // Rather than explicitly coding a Waterfall we have only to declare what properties we
        // want collected.
        // In this example we will want two text prompts to run, one for the first name and one
        // for the last.
        List<SlotDetails> fullname_slots = Arrays.asList(
            new SlotDetails("first", "text", "Please enter your first name."),
            new SlotDetails("last", "text", "Please enter your last name.")
        );

        // This defines an address dialog that collects street, city and zip properties.
        List<SlotDetails> address_slots = Arrays.asList(
            new SlotDetails("street", "text", "Please enter the street."),
            new SlotDetails("city", "text", "Please enter the city."),
            new SlotDetails("zip", "text", "Please enter the zip.")
        );

        // Dialogs can be nested and the slot filling dialog makes use of that. In this example
        // some of the child dialogs are slot filling dialogs themselves.
        List<SlotDetails> slots = Arrays.asList(
            new SlotDetails("fullname", "fullname"),
            new SlotDetails("age", "number", "Please enter your age."),
            new SlotDetails(
                "shoesize", "shoesize", "Please enter your shoe size.",
                "You must enter a size between 0 and 16. Half sizes are acceptable."
            ),
            new SlotDetails("address", "address")
        );

        // Add the various dialogs that will be used to the DialogSet.
        addDialog(new SlotFillingDialog("address", address_slots));
        addDialog(new SlotFillingDialog("fullname", fullname_slots));
        addDialog(new TextPrompt("text"));
        addDialog(new NumberPrompt<>("number", Integer.class));
        addDialog(new NumberPrompt<Float>("shoesize", this::shoeSize, Culture.English, Float.class));
        addDialog(new SlotFillingDialog("slot-dialog", slots));

        // Defines a simple two step Waterfall to test the slot dialog.
        WaterfallStep[] waterfallSteps = {
            this::startDialog,
            this::processResults
        };
        addDialog(new WaterfallDialog("waterfall", Arrays.asList(waterfallSteps)));

        // The initial child Dialog to run.
        setInitialDialogId("waterfall");
    }

    private CompletableFuture<Boolean> shoeSize(PromptValidatorContext<Float> promptContext) {
        Float shoesize = promptContext.getRecognized().getValue();

        // show sizes can range from 0 to 16
        if (shoesize >= 0 && shoesize <= 16) {
            // we only accept round numbers or half sizes
            if (Math.floor(shoesize) == shoesize || Math.floor(shoesize * 2) == shoesize * 2) {
                // indicate success by returning the value
                return CompletableFuture.completedFuture(true);
            }
        }

        return CompletableFuture.completedFuture(false);
    }

    private CompletableFuture<DialogTurnResult> startDialog(WaterfallStepContext stepContext) {
        // Start the child dialog. This will run the top slot dialog than will complete when
        // all the properties are gathered.
        return stepContext.beginDialog("slot-dialog");
    }

    private CompletableFuture<DialogTurnResult> processResults(WaterfallStepContext stepContext) {
        Map<String, Object> result = stepContext.getResult() instanceof Map
            ? (Map<String, Object>) stepContext.getResult()
            : null;

        // To demonstrate that the slot dialog collected all the properties we will echo them back to the user.
        if (result != null && result.size() > 0) {
            // Now the waterfall is complete, save the data we have gathered into UserState.
            userStateAccessor.get(stepContext.getContext(), HashMap::new)
                .thenApply(obj -> {
                    Map<String, Object> fullname = (Map<String, Object>) result.get("fullname");
                    Float shoesize = (Float) result.get("shoesize");
                    Map<String, Object> address = (Map<String, Object>) result.get("address");

                    Map<String, String> data = new HashMap<>();
                    data.put("fullname", String.format("%s %s", fullname.get("first"), fullname.get("last")));
                    data.put("shoesize", Float.toString(shoesize));
                    data.put("address", String.format("%s, %s, %s", address.get("street"), address.get("city"), address.get("zip")));

                    obj.put("data", data);
                    return obj;
                })
                .thenCompose(obj -> stepContext.getContext().sendActivities(
                    MessageFactory.text(obj.get("data").get("fullname")),
                    MessageFactory.text(obj.get("data").get("shoesize")),
                    MessageFactory.text(obj.get("data").get("address"))
                ));
        }

        // Remember to call EndAsync to indicate to the runtime that this is the end of our waterfall.
        return stepContext.endDialog();
    }
}
