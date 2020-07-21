#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os


class DefaultConfig:
    """ Bot Configuration """

    PORT = 3978
    APP_ID = os.environ.get("MicrosoftAppId", "4bad5254-eb4c-45d5-9a70-4dccb611df46")
    APP_PASSWORD = os.environ.get("MicrosoftAppPassword", ".9A1p~AO6mWj4jJU5_st_Q~Vo9enBvl9Wn")
    BASE_URL = os.environ.get("BaseUrl", "https://trboehrePyTeamsTasks.azurewebsites.net")
