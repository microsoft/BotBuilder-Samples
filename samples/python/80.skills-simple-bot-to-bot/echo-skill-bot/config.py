#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os


class DefaultConfig:
    """ Bot Configuration """

    PORT = 39783
    APP_ID = os.environ.get("MicrosoftAppId", "TODO: Add here the App ID for the skill bot")
    APP_PASSWORD = os.environ.get("MicrosoftAppPassword", "TODO: Add here the App Password for the skill bot")
    
    # Callers to only those specified, '*' allows any caller.
    # Example: os.environ.get("AllowedCallers", ["aaaaaa-1111-1111-1111-aaaaaaaaaa"])
    ALLOWED_CALLERS = os.environ.get("AllowedCallers", ["*"])
