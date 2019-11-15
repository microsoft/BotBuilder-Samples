#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os

""" Bot Configuration """


class DefaultConfig:
    """ Bot Configuration """

    PORT = 3978
    APP_ID = os.environ.get("MicrosoftAppId", "")
    APP_PASSWORD = os.environ.get("MicrosoftAppPassword", "")
    BLOB_ACCOUNT_NAME = "tboehrestorage"
    BLOB_KEY = "A7tc3c9T/n67iDYO7Lx19sTjnA+DD3bR/HQ4yPhJuyVXO1yJ8mYzDOXsBhJrjldh7zKMjE9Wc6PrM1It4nlGPw=="
    BLOB_CONTAINER = "dialogs"
