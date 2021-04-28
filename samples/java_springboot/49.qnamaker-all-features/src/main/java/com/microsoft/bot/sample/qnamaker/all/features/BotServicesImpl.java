// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.qnamaker.all.features;

import com.microsoft.bot.ai.qna.QnAMaker;
import com.microsoft.bot.ai.qna.QnAMakerEndpoint;
import com.microsoft.bot.integration.Configuration;
import org.apache.commons.lang3.StringUtils;

/**
 * Class which implements the BotService interface to handle
 * the services attached to the bot.
 */
public class BotServicesImpl implements BotServices {

    private QnAMaker qnAMakerService;

    /**
     * Initializes a new instance of the {@link BotServicesImpl} class.
     *
     * @param configuration A {@link Configuration} which contains the properties of the application.properties
     */
    public BotServicesImpl(Configuration configuration) {
        QnAMakerEndpoint qnAMakerEndpoint = new QnAMakerEndpoint();
        qnAMakerEndpoint.setKnowledgeBaseId(configuration.getProperty("QnAKnowledgebaseId"));
        qnAMakerEndpoint.setHost(getHostname(configuration.getProperty("QnAEndpointHostName")));
        qnAMakerEndpoint.setEndpointKey(getEndpointKey(configuration));
        QnAMaker qnAMaker = new QnAMaker(qnAMakerEndpoint, null);
        this.qnAMakerService = qnAMaker;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public QnAMaker getQnAMakerService() {
        return qnAMakerService;
    }

    /**
     * Get the URL with the hostname value.
     *
     * @param hostname The hostname value
     * @return The URL with the hostname value
     */
    private static String getHostname(String hostname) {
        if (!hostname.startsWith("https://")) {
            hostname = "https://".concat(hostname);
        }

        if (!hostname.contains("/v5.0") && !hostname.endsWith("/qnamaker")) {
            hostname = hostname.concat("/qnamaker");
        }

        return hostname;
    }

    /**
     * Get the endpointKey.
     *
     * @param configuration A {@link Configuration} populated with the application.properties file
     * @return The endpointKey
     */
    private static String getEndpointKey(Configuration configuration) {
        String endpointKey = configuration.getProperty("QnAEndpointKey");

        if (StringUtils.isBlank(endpointKey)) {
            // This features sample is copied as is for "azure bot service" default "createbot" template.
            // Post this sample change merged into "azure bot service" template repo, "Azure Bot Service"
            // will make the web app config change to use "QnAEndpointKey".But, the the old "QnAAuthkey"
            // required for backward compact. This is a requirement from docs to keep app setting name
            // consistent with "QnAEndpointKey". This is tracked in Github issue:
            // https://github.com/microsoft/BotBuilder-Samples/issues/2532

            endpointKey = configuration.getProperty("QnAAuthKey");
        }

        return endpointKey;
    }
}
