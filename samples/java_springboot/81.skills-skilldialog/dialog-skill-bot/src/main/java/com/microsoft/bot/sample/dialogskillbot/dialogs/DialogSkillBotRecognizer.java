// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogskillbot.dialogs;

import java.util.concurrent.CompletableFuture;

import com.microsoft.bot.ai.luis.LuisApplication;
import com.microsoft.bot.ai.luis.LuisRecognizer;
import com.microsoft.bot.ai.luis.LuisRecognizerOptionsV3;
import com.microsoft.bot.builder.Recognizer;
import com.microsoft.bot.builder.RecognizerConvert;
import com.microsoft.bot.builder.RecognizerResult;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.integration.Configuration;

import org.apache.commons.lang3.StringUtils;

public class DialogSkillBotRecognizer implements Recognizer {

    private final LuisRecognizer _recognizer;
    public DialogSkillBotRecognizer(Configuration configuration) {
        boolean luisIsConfigured = !StringUtils.isAllBlank(configuration.getProperty("LuisAppId"))
                                   && !StringUtils.isAllBlank(configuration.getProperty("LuisAPIKey"))
                                   && !StringUtils.isAllBlank(configuration.getProperty("LuisAPIHostName"));
        if (luisIsConfigured) {
            LuisApplication luisApplication = new LuisApplication(
                configuration.getProperty("LuisAppId"),
                configuration.getProperty("LuisAPIKey"),
                String.format("https://%s", configuration.getProperty("LuisAPIHostName")));
            LuisRecognizerOptionsV3 luisOptions = new LuisRecognizerOptionsV3(luisApplication);

            _recognizer = new LuisRecognizer(luisOptions);
        } else {
            _recognizer = null;
        }
    }

    // Returns true if LUS instanceof configured in the appsettings.json and initialized.
    public boolean getIsConfigured() {
        return _recognizer != null;
    }

    public CompletableFuture<RecognizerResult> recognize(TurnContext turnContext) {
        return _recognizer.recognize(turnContext);
    }

    public <T extends RecognizerConvert> CompletableFuture<T> recognize(
        TurnContext turnContext,
        Class<T> c
    ) {
        return _recognizer.recognize(turnContext, c);
    }
}
