// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogrootbot;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.HashMap;
import java.util.Map;

import com.microsoft.bot.builder.skills.BotFrameworkSkill;
import com.microsoft.bot.integration.Configuration;

import org.apache.commons.lang3.StringUtils;

/**
 * A helper class that loads Skills information from configuration.
 */
public class SkillsConfiguration {

    private URI skillHostEndpoint;

    private Map<String, BotFrameworkSkill> skills = new HashMap<String, BotFrameworkSkill>();

    public SkillsConfiguration(Configuration configuration) {

        boolean noMoreEntries = false;
        int indexCount = 0;
        while (!noMoreEntries) {
            String botID = configuration.getProperty(String.format("BotFrameworkSkills[%d].Id", indexCount));
            String botAppId = configuration.getProperty(String.format("BotFrameworkSkills[%d].AppId", indexCount));
            String skillEndPoint =
                configuration.getProperty(String.format("BotFrameworkSkills[%d].SkillEndpoint", indexCount));
            if (
                StringUtils.isNotBlank(botID) && StringUtils.isNotBlank(botAppId)
                    && StringUtils.isNotBlank(skillEndPoint)
            ) {
                BotFrameworkSkill newSkill = new BotFrameworkSkill();
                newSkill.setId(botID);
                newSkill.setAppId(botAppId);
                try {
                    newSkill.setSkillEndpoint(new URI(skillEndPoint));
                } catch (URISyntaxException e) {
                    e.printStackTrace();
                }
                skills.put(botID, newSkill);
                indexCount++;
            } else {
                noMoreEntries = true;
            }
        }

        String skillHost = configuration.getProperty("SkillhostEndpoint");
        if (!StringUtils.isEmpty(skillHost)) {
            try {
                skillHostEndpoint = new URI(skillHost);
            } catch (URISyntaxException e) {
                e.printStackTrace();
            }
        }
    }

    /**
     * @return the SkillHostEndpoint value as a Uri.
     */
    public URI getSkillHostEndpoint() {
        return this.skillHostEndpoint;
    }

    /**
     * @return the Skills value as a Dictionary<String, BotFrameworkSkill>.
     */
    public Map<String, BotFrameworkSkill> getSkills() {
        return this.skills;
    }

}
