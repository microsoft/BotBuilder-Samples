#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os


class DefaultConfig:
    """ Bot Configuration """

    PORT = 3978
    APP_ID = os.environ.get("MicrosoftAppId", "fb33674c-a6b0-431e-8538-8dc27dc0bea6")
    APP_PASSWORD = os.environ.get("MicrosoftAppPassword", "4iXtf6EFo9ofmjAHg-YeL._8Ut4rN-LHR4") #Dev-1y

