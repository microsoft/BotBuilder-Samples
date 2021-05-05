// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.facebookevents;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.connector.Channels;
import com.microsoft.bot.dialogs.choices.Choice;
import com.microsoft.bot.dialogs.choices.ChoiceFactory;
import com.microsoft.bot.schema.ActionTypes;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.CardAction;
import com.microsoft.bot.schema.HeroCard;
import com.microsoft.bot.schema.Serialization;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 */
public class FacebookBot extends ActivityHandler {

    // These are the options provided to the user when they message the bot
    final String FacebookPageIdOption = "Facebook Id";
    final String QuickRepliesOption = "Quick Replies";
    final String PostBackOption = "PostBack";

    protected final Logger Logger;

    public FacebookBot() {
        Logger = LoggerFactory.getLogger(FacebookBot.class);
    }

    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        Logger.info("Processing a Message Activity.");

        // Show choices if the Facebook Payload from ChannelData instanceof not handled
        try {
            return processFacebookPayload(turnContext, turnContext.getActivity().getChannelData())
                .thenCompose(result -> {
                    if (result == false) {
                        return showChoices(turnContext);
                    } else {
                        return CompletableFuture.completedFuture(null);
                    }
                });
        } catch (JsonProcessingException e) {
            e.printStackTrace();
            return CompletableFuture.completedFuture(null);
        }
    }

    @Override
    protected CompletableFuture<Void> onEventActivity(TurnContext turnContext) {
        Logger.info("Processing an Event Activity.");

        // Analyze Facebook payload from EventActivity.Value
        try {
            return processFacebookPayload(turnContext, turnContext.getActivity().getValue()).thenApply(result -> null);
        } catch (JsonProcessingException e) {
            e.printStackTrace();
            return CompletableFuture.completedFuture(null);
        }
    }

    private CompletableFuture<Void> showChoices(TurnContext turnContext) {
        // Create choices for the prompt
        List<Choice> choices = new ArrayList<Choice>();

        Choice firstChoice = new Choice();
        firstChoice.setValue(QuickRepliesOption);
        CardAction firstChoiceCardAction = new CardAction();
        firstChoiceCardAction.setTitle(QuickRepliesOption);
        firstChoiceCardAction.setType(ActionTypes.POST_BACK);
        firstChoiceCardAction.setValue(QuickRepliesOption);
        firstChoice.setAction(firstChoiceCardAction);
        choices.add(firstChoice);

        Choice secondChoice = new Choice();
        firstChoice.setValue(FacebookPageIdOption);
        CardAction secondChoiceCardAction = new CardAction();
        secondChoiceCardAction.setTitle(FacebookPageIdOption);
        secondChoiceCardAction.setType(ActionTypes.POST_BACK);
        secondChoiceCardAction.setValue(FacebookPageIdOption);
        secondChoice.setAction(secondChoiceCardAction);
        choices.add(secondChoice);

        Choice thirdChoice = new Choice();
        firstChoice.setValue(PostBackOption);
        CardAction thirdChoiceCardAction = new CardAction();
        thirdChoiceCardAction.setTitle(PostBackOption);
        thirdChoiceCardAction.setType(ActionTypes.POST_BACK);
        thirdChoiceCardAction.setValue(PostBackOption);
        thirdChoice.setAction(thirdChoiceCardAction);

        choices.add(thirdChoice);

        // Create the prompt message
        Activity message = ChoiceFactory.forChannel(
            turnContext.getActivity().getChannelId(),
            choices,
            "What Facebook feature would you like to try? Here are some quick replies to choose from!"
        );
        return turnContext.sendActivity(message).thenApply(result -> null);
    }


    private CompletableFuture<Boolean> processFacebookPayload(
        TurnContext turnContext,
        Object data
    ) throws JsonProcessingException {
        // At this point we know we are on Facebook channel, and can consume the
        // Facebook custom payload
        // present in channelData.
        try {
            FacebookPayload facebookPayload = Serialization.safeGetAs(data, FacebookPayload.class);

            if (facebookPayload != null) {
                // PostBack
                if (facebookPayload.getPostBack() != null) {
                    return onFacebookPostBack(turnContext, facebookPayload.getPostBack()).thenApply(result -> true);
                } else if (facebookPayload.getOptin() != null) {
                    // Optin
                    return onFacebookOptin(turnContext, facebookPayload.getOptin()).thenApply(result -> true);
                } else if (
                    facebookPayload.getMessage() != null && facebookPayload.getMessage().getQuickReply() != null
                ) {
                    // Quick reply
                    return onFacebookQuickReply(turnContext, facebookPayload.getMessage().getQuickReply())
                        .thenApply(result -> true);
                } else if (facebookPayload.getMessage() != null && facebookPayload.getMessage().getIsEcho()) {
                    // Echo
                    return onFacebookEcho(turnContext, facebookPayload.getMessage()).thenApply(result -> true);
                } else {
                    return turnContext.sendActivity("This sample is intended to be used with a Facebook bot.")
                           .thenApply(result -> false);
                }

                // TODO: Handle other events that you're interested in...
            }
        } catch (JsonProcessingException ex) {
            if (turnContext.getActivity().getChannelId() != Channels.FACEBOOK) {
                turnContext.sendActivity("This sample is intended to be used with a Facebook bot.");
            } else {
                throw ex;
            }
        }

        return CompletableFuture.completedFuture(false);
    }

    protected CompletableFuture<Void> onFacebookOptin(TurnContext turnContext, FacebookOptin optin) {
        Logger.info("Optin message received.");

        // TODO: Your optin event handling logic here...
        return CompletableFuture.completedFuture(null);
    }

    protected CompletableFuture<Void> onFacebookEcho(TurnContext turnContext, FacebookMessage facebookMessage) {
        Logger.info("Echo message received.");

        // TODO: Your echo event handling logic here...
        return CompletableFuture.completedFuture(null);
    }

    protected CompletableFuture<Void> onFacebookPostBack(TurnContext turnContext, FacebookPostback postBack) {
        Logger.info("PostBack message received.");

        // TODO: Your PostBack handling logic here...

        // Answer the postback, and show choices
        Activity reply = MessageFactory.text("Are you sure?");
        return turnContext.sendActivity(reply).thenCompose(result -> {
            return showChoices(turnContext);
        });
    }

    protected CompletableFuture<Void> onFacebookQuickReply(TurnContext turnContext, FacebookQuickReply quickReply) {
        Logger.info("QuickReply message received.");

        // TODO: Your quick reply event handling logic here...

        // Process the message by checking the Activity.getText(). The
        // FacebookQuickReply could also contain a json payload.

        // Initially the bot offers to showcase 3 Facebook features: Quick replies,
        // PostBack and getting the Facebook Page Name.
        switch (turnContext.getActivity().getText()) {
            // Here we showcase how to obtain the Facebook page id.
            // This can be useful for the Facebook multi-page support provided by the Bot
            // Framework.
            // The Facebook page id from which the message comes from instanceof in
            // turnContext.getActivity().getRecipient().getId().
            case FacebookPageIdOption: {
                Activity reply = MessageFactory.text(
                    String.format(
                        "This message comes from the following Facebook Page: %s",
                        turnContext.getActivity().getRecipient().getId()
                    )
                );
                return turnContext.sendActivity(reply).thenCompose(result -> {
                    return showChoices(turnContext);
                });
            }

            // Here we send a HeroCard with 2 options that will trigger a Facebook PostBack.
            case PostBackOption: {
                HeroCard card = new HeroCard();
                card.setText("Is 42 the answer to the ultimate question of Life, the Universe, and Everything?");

                List<CardAction> buttons = new ArrayList<CardAction>();

                CardAction yesAction = new CardAction();
                yesAction.setTitle("Yes");
                yesAction.setType(ActionTypes.POST_BACK);
                yesAction.setValue("Yes");
                buttons.add(yesAction);

                CardAction noAction = new CardAction();
                noAction.setTitle("No");
                noAction.setType(ActionTypes.POST_BACK);
                noAction.setValue("No");
                buttons.add(noAction);

                card.setButtons(buttons);

                Activity reply = MessageFactory.attachment(card.toAttachment());
                return turnContext.sendActivity(reply).thenApply(result -> null);
            }

            // By default we offer the users different actions that the bot supports,
            // through quick replies.
            case QuickRepliesOption:
            default: {
                return showChoices(turnContext);
            }
        }
    }
}
