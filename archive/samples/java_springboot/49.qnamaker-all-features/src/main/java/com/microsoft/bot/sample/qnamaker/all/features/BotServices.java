// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.qnamaker.all.features;

import com.microsoft.bot.ai.qna.QnAMaker;

/**
 * Interface which contains the QnAMaker application.
 */
public interface BotServices {

    /**
     * Get QnAMaker application.
     *
     * @return QnAMaker application
     */
    QnAMaker getQnAMakerService();
}
