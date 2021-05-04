// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.customdialogs;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.prompts.PromptOptions;

/**
 * A list of SlotDetails defines the behavior of our SlotFillingDialog. This class represents a
 * description of a single 'slot'. It contains the name of the property we want to gather and the id
 * of the corresponding dialog that should be used to gather that property. The id is that used when
 * the dialog was added to the current DialogSet. Typically this id is that of a prompt but it could
 * also be the id of another slot dialog.
 */
public class SlotDetails {

    private String name;
    private String dialogId;
    private PromptOptions options;

    public SlotDetails(
        String withName, String withDialogId, String withPrompt
    ) {
        this(withName, withDialogId, withPrompt, null);
    }

    public SlotDetails(
        String withName, String withDialogId, String withPrompt, String withRetryPrompt
    ) {
        name = withName;
        dialogId = withDialogId;
        options = new PromptOptions();
        options.setPrompt(MessageFactory.text(withPrompt));
        options.setRetryPrompt(MessageFactory.text(withRetryPrompt));
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getDialogId() {
        return dialogId;
    }

    public void setDialogId(String withDialogId) {
        dialogId = withDialogId;
    }

    public PromptOptions getOptions() {
        return options;
    }

    public void setOptions(PromptOptions withOptions) {
        options = withOptions;
    }
}
