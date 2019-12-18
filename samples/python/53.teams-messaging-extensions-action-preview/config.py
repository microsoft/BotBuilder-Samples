#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os


class DefaultConfig:
    """ Bot Configuration """

    PORT = 3978
    APP_ID = os.environ.get("MicrosoftAppId", "3851a47b-53ed-4d29-b878-6e941da61e98")
    APP_PASSWORD = os.environ.get(
        "MicrosoftAppPassword", "CUW5X6][SIR:]ttG2oMkTMAd24hQey2@"
    )
