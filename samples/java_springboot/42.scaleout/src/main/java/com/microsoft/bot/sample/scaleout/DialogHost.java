// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.scaleout;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.JsonNodeFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.TurnContextImpl;
import com.microsoft.bot.connector.Async;
import com.microsoft.bot.dialogs.Dialog;
import com.microsoft.bot.dialogs.DialogState;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.Pair;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.concurrent.CompletableFuture;

/**
 * The essential code for running a dialog. The execution of the dialog is treated here as a pure function call.
 * The input being the existing (or old) state and the inbound Activity and the result being the updated (or new) state
 * and the Activities that should be sent. The assumption is that this code can be re-run without causing any
 * unintended or harmful side-effects, for example, any outbound service calls made directly from the
 * dialog implementation should be idempotent.
 */
public final class DialogHost {

    /**
     * The... ummm... logger.
     */
    private static Logger logger = LoggerFactory.getLogger(DialogHost.class);

    // The serializer to use. Moving the serialization to this layer will make the storage layer more pluggable.
    private static ObjectMapper objectMapper = new ObjectMapper()
        .configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false)
        .findAndRegisterModules()
        .enableDefaultTyping();

    private DialogHost() { }

    /**
     * A function to run a dialog while buffering the outbound Activities.
     *
     * @param dialog The dialog to run.
     * @param activity The inbound Activity to run it with.
     * @param oldState The existing or old state.
     * @return An array of Activities 'sent' from the dialog as it executed. And the updated or new state.
     */
    public static CompletableFuture<Pair<Activity[], JsonNode>> run(
        Dialog dialog,
        Activity activity,
        JsonNode oldState) {
        // A custom adapter and corresponding TurnContext that buffers any messages sent.
        DialogHostAdapter adapter = new DialogHostAdapter();
        TurnContext turnContext = new TurnContextImpl(adapter, activity);

        // Run the dialog using this TurnContext with the existing state.
        return runTurn(dialog, turnContext, oldState)
            .thenApply(newState -> new Pair<>(
                (adapter.getResponses().toArray(new Activity[adapter.getResponses().size()])),
                newState));
    }

    /**
     * Execute the turn of the bot. The functionality here closely resembles that which is found in the
     * IBot.OnTurnAsync method in an implementation that is using the regular BotFrameworkAdapter.
     * Also here in this example the focus is explicitly on Dialogs but the pattern could be adapted
     * to other conversation modeling abstractions.
     *
     * @param dialog The dialog to be run.
     * @param turnContext The ITurnContext instance to use. Note this is not the one passed into the IBot OnTurnAsync.
     * @param state The existing or old state of the dialog.
     * @return The updated or new state of the dialog.
     */
    private static CompletableFuture<JsonNode> runTurn(Dialog dialog, TurnContext turnContext, JsonNode state) {
        // If we have some satte, desearlize it. (This mimics the sape produced by BotState.java)
        JsonNode dialogStateProperty = null;
        if (state != null) {
            dialogStateProperty = state.get("DialogState");
        }
        DialogState dialogState;
        try {
            Class<?> cls = Class.forName(DialogState.class.getName());
            dialogState = (DialogState) objectMapper.treeToValue(dialogStateProperty, cls);
        } catch (JsonProcessingException e) {
            logger.error("RunTurn failed: {}", e.toString());
            return Async.completeExceptionally(new RuntimeException(
                String.format("RunTurn failed: %s", e.toString())
            ));
        } catch (ClassNotFoundException e) {
            logger.error("RunTurn failed: Could not load class DialogState");
            return Async.completeExceptionally(new RuntimeException(
                "RunTurn failed: Could not load class DialogState"
            ));
        }

        // A custom accessor is used to pass a handle on the state to the dialog system.
        RefAccessor accessor = new RefAccessor<DialogState>(dialogState);

        // Run the dialog.
        return Dialog.run(dialog, turnContext, accessor)
            .thenApply(result -> {
                // Serialize the result (available as Value on the accessor),
                // and put its value back into a new JsonNode.
                return JsonNodeFactory
                    .instance
                    .objectNode().set("DialogState", objectMapper.valueToTree(accessor.getValue()));
            });
    }
}
