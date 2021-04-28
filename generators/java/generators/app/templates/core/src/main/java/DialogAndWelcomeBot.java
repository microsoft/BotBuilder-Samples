// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package <%= packageName %>;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.microsoft.applicationinsights.core.dependencies.apachecommons.io.IOUtils;
import com.microsoft.applicationinsights.core.dependencies.apachecommons.lang3.StringUtils;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.dialogs.Dialog;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.Attachment;
import com.microsoft.bot.schema.ChannelAccount;
import com.microsoft.bot.schema.Serialization;

import java.io.IOException;
import java.io.InputStream;
import java.nio.charset.StandardCharsets;
import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * The class containing the welcome dialog.
 *
 * @param <T> is a Dialog.
 */
public class DialogAndWelcomeBot<T extends Dialog> extends DialogBot {

    /**
     * Creates a DialogBot.
     *
     * @param withConversationState ConversationState to use in the bot
     * @param withUserState         UserState to use
     * @param withDialog            Param inheriting from Dialog class
     */
    public DialogAndWelcomeBot(
        ConversationState withConversationState, UserState withUserState, T withDialog
    ) {
        super(withConversationState, withUserState, withDialog);
    }

    /**
     * When the {@link #onConversationUpdateActivity(TurnContext)} method receives a conversation
     * update activity that indicates one or more users other than the bot are joining the
     * conversation, it calls this method.
     *
     * @param membersAdded A list of all the members added to the conversation, as described by the
     *                     conversation update activity
     * @param turnContext  The context object for this turn.
     * @return A task that represents the work queued to execute.
     */
    @Override
    protected CompletableFuture<Void> onMembersAdded(
        List<ChannelAccount> membersAdded, TurnContext turnContext
    ) {
        return turnContext.getActivity().getMembersAdded().stream()
            .filter(member -> !StringUtils
                .equals(member.getId(), turnContext.getActivity().getRecipient().getId()))
            .map(channel -> {
                // Greet anyone that was not the target (recipient) of this message.
                // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                Attachment welcomeCard = createAdaptiveCardAttachment();
                Activity response = MessageFactory
                    .attachment(welcomeCard, null, "Welcome to Bot Framework!", null);

                return turnContext.sendActivity(response).thenApply(sendResult -> {
                    return Dialog.run(getDialog(), turnContext,
                        getConversationState().createProperty("DialogState")
                    );
                });
            })
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponse -> null);
    }

    // Load attachment from embedded resource.
    private Attachment createAdaptiveCardAttachment() {
        try (
            InputStream inputStream = Thread.currentThread().
                getContextClassLoader().getResourceAsStream("cards/welcomeCard.json")
        ) {
            String adaptiveCardJson = IOUtils
                .toString(inputStream, StandardCharsets.UTF_8.toString());

            Attachment attachment = new Attachment();
            attachment.setContentType("application/vnd.microsoft.card.adaptive");
            attachment.setContent(Serialization.jsonToTree(adaptiveCardJson));
            return attachment;

        } catch (IOException e) {
            e.printStackTrace();
            return new Attachment();
        }
    }
}
