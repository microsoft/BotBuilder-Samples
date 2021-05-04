// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.qnamaker.all.features;

import com.microsoft.bot.ai.qna.QnADialogResponseOptions;
import com.microsoft.bot.ai.qna.QnAMakerClient;
import com.microsoft.bot.ai.qna.QnAMakerOptions;
import com.microsoft.bot.ai.qna.dialogs.QnAMakerDialog;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.DialogContext;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.schema.Activity;
import org.apache.commons.lang3.StringUtils;

import java.util.concurrent.CompletableFuture;

/**
 * QnAMaker action builder class.
 */
public class QnAMakerBaseDialog extends QnAMakerDialog {

    // Dialog Options parameters
    private final String defaultCardTitle = "Did you mean:";
    private final String defaultCardNoMatchText = "None of the above.";
    private final String defaultCardNoMatchResponse = "Thanks for the feedback.";

    private final BotServices services;
    private String defaultAnswer = "No QnAMaker answer found.";

    /**
     * Initializes a new instance of the {@link QnAMakerBaseDialog} class.
     * Dialog helper to generate dialogs.
     *
     * @param withServices Bot Services
     * @param configuration A {@link Configuration} which contains the properties of the application.properties
     */
    public QnAMakerBaseDialog(BotServices withServices, Configuration configuration) {
        super();
        this.services = withServices;
        if (StringUtils.isNotBlank(configuration.getProperty("DefaultAnswer"))) {
            this.defaultAnswer = configuration.getProperty("DefaultAnswer");
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    protected CompletableFuture<QnAMakerClient> getQnAMakerClient(DialogContext dc) {
        return CompletableFuture.completedFuture(this.services.getQnAMakerService());
    }

    /**
     * {@inheritDoc}
     */
    @Override
    protected CompletableFuture<QnAMakerOptions> getQnAMakerOptions(DialogContext dc) {
        QnAMakerOptions options = new QnAMakerOptions();
        options.setScoreThreshold(DEFAULT_THRESHOLD);
        options.setTop(DEFAULT_TOP_N);
        options.setQnAId(0);
        options.setRankerType("Default");
        options.setIsTest(false);
        return CompletableFuture.completedFuture(options);
    }

    /**
     * {@inheritDoc}
     */
    @Override
    protected CompletableFuture<QnADialogResponseOptions> getQnAResponseOptions(DialogContext dc) {
        Activity defaultAnswerActivity = MessageFactory.text(this.defaultAnswer);

        Activity cardNoMatchResponse = MessageFactory.text(this.defaultCardNoMatchResponse);

        QnADialogResponseOptions responseOptions = new QnADialogResponseOptions();
        responseOptions.setActiveLearningCardTitle(this.defaultCardTitle);
        responseOptions.setCardNoMatchText(this.defaultCardNoMatchText);
        responseOptions.setNoAnswer(defaultAnswerActivity);
        responseOptions.setCardNoMatchResponse(cardNoMatchResponse);

        return CompletableFuture.completedFuture(responseOptions);
    }
}
