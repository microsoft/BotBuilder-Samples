#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os

""" Bot Configuration """


class DefaultConfig:
    """ Bot Configuration """

    PORT = 39783
    APP_ID = os.environ.get("MicrosoftAppId", "TODO: Add here the App ID for the skill bot")
    APP_PASSWORD = os.environ.get("MicrosoftAppPassword", "TODO: Add here the App Password for the skill bot")
    
    # If ALLOWED_CALLERS is empty, any bot can call this Skill.  Add MicrosoftAppIds to restrict callers to only those specified.
    # Example: os.environ.get("AllowedCallers", ["54d3bb6a-3b6d-4ccd-bbfd-cad5c72fb53a", "3851a47b-53ed-4d29-b878-6e941da61e98"])
    ALLOWED_CALLERS = os.environ.get("AllowedCallers", [])
