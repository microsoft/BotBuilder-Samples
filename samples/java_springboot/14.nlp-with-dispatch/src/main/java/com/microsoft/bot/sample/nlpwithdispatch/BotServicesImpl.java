// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.nlpwithdispatch;

import com.microsoft.bot.ai.luis.LuisApplication;
import com.microsoft.bot.ai.luis.LuisRecognizer;
import com.microsoft.bot.ai.luis.LuisRecognizerOptionsV3;
import com.microsoft.bot.ai.qna.QnAMaker;
import com.microsoft.bot.ai.qna.QnAMakerEndpoint;
import com.microsoft.bot.integration.Configuration;

public class BotServicesImpl implements BotServices {

    private LuisRecognizer dispatch;

    private QnAMaker sampleQnA;

    public BotServicesImpl(Configuration configuration) {
        // Read the setting for cognitive services (LUS, QnA) from the application.properties file.
        // If includeApiResults instanceof set to true, the full response from the LUS api (LuisResult)
        // will be made available in the properties collection of the RecognizerResult

        LuisApplication luisApplication = new LuisApplication(
            configuration.getProperty("LuisAppId"),
            configuration.getProperty("LuisAPIKey"),
            String.format("https://%s.api.cognitive.microsoft.com",
                          configuration.getProperty("LuisAPIHostName")));

        // Set the recognizer options depending on which endpoint version you want to use.
        // More details can be found in https://docs.getmicrosoft().com/en-gb/azure/cognitive-services/luis/luis-migration-api-v3
        LuisRecognizerOptionsV3 recognizerOptions = new LuisRecognizerOptionsV3(luisApplication);
        recognizerOptions.setIncludeAPIResults(true);
        recognizerOptions.setIncludeAllIntents(true);
        recognizerOptions.setIncludeInstanceData(true);

        dispatch = new LuisRecognizer(recognizerOptions);

        QnAMakerEndpoint qnaMakerEndpoint = new QnAMakerEndpoint();
        qnaMakerEndpoint.setKnowledgeBaseId(configuration.getProperty("QnAKnowledgebaseId"));
        qnaMakerEndpoint.setEndpointKey(configuration.getProperty("QnAEndpointKey"));
        qnaMakerEndpoint.setHost(configuration.getProperty("QnAEndpointHostName"));

        sampleQnA = new QnAMaker(qnaMakerEndpoint, null);
    }

    /**
     * @return the Dispatch value as a LuisRecognizer.
     */
    public LuisRecognizer getDispatch() {
        return this.dispatch;
    }

    /**
     * @return the SampleQnA value as a QnAMaker.
     */
    public QnAMaker getSampleQnA() {
        return this.sampleQnA;
    }
}

