#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os
from typing import Dict
from botbuilder.core.skills import BotFrameworkSkill


class DefaultConfig:
    """ Bot Configuration """

    PORT = 3978
    APP_ID = os.environ.get("MicrosoftAppId", "TODO: Add here the App ID for the bot")
    APP_PASSWORD = os.environ.get("MicrosoftAppPassword", "TODO: Add here the password for the bot")
    SKILL_HOST_ENDPOINT = "http://localhost:3978/api/skills"
    SKILLS = [
        {
            "id": "DialogSkillBot",
            "app_id": "TODO: Add here the App ID for the skill",
            "skill_endpoint": "http://localhost:39783/api/messages",
        },
    ]


class SkillConfiguration:
    SKILL_HOST_ENDPOINT = DefaultConfig.SKILL_HOST_ENDPOINT
    SKILLS: Dict[str, BotFrameworkSkill] = {
        skill["id"]: BotFrameworkSkill(**skill) for skill in DefaultConfig.SKILLS
    }
