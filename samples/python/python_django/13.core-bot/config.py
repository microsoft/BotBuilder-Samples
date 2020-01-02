#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

""" Bot Configuration """


class DefaultConfig(object):
    """ Bot Configuration """

    PORT = 3978
    APP_ID = ""
    APP_PASSWORD = ""

    LUIS_APP_ID = ""
    # LUIS authoring key from LUIS portal or LUIS Cognitive Service subscription key
    LUIS_API_KEY = ""
    # LUIS endpoint host name, ie "https://westus.api.cognitive.microsoft.com"
    LUIS_API_HOST_NAME = ""
