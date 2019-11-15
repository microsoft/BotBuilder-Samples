#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Bot/Flask Configuration parameters.
Configuration parameters for the bot.
"""


class DefaultConfig(object):
    """Bot configuration parameters."""

    # TCP port that the bot listens on (default:3978)
    PORT = 3978

    # Azure Application ID (not required if running locally)
    APP_ID = ""
    # Azure Application Password (not required if running locally)
    APP_PASSWORD = ""

    # Determines if the bot calls the models in-proc to the bot or call out of process
    # to the service api.
    USE_MODEL_RUNTIME_SERVICE = False
    # Host serving the out-of-process model runtime service api.
    MODEL_RUNTIME_SERVICE_HOST = "localhost"
    # TCP serving the out-of-process model runtime service api.
    MODEL_RUNTIME_SERVICE_PORT = 8880
