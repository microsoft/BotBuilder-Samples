#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os


class DefaultConfig:
    """ Bot Configuration """

    PORT = 3978

    SLACK_VERIFICATION_TOKEN = os.environ.get("SlackVerificationToken", "")
    SLACK_BOT_TOKEN = os.environ.get("SlackBotToken", "")
    SLACK_CLIENT_SIGNING_SECRET = os.environ.get("SlackClientSigningSecret", "")
