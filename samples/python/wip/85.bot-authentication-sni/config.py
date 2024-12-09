#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import os

""" Bot Configuration """


class DefaultConfig:
    """ Bot Configuration """

    PORT = 3978
    APP_TYPE = os.environ.get("MicrosoftAppType", "MultiTenant")
    APP_ID = os.environ.get("MicrosoftAppId", "")
    APP_PASSWORD = os.environ.get("MicrosoftAppPassword", "")
    APP_TENANTID = os.environ.get("MicrosoftAppTenantId", "")
    APP_KEYVAULTNAME = os.environ.get("MicrosoftAppKeyVaultName", "")
    APP_CERTIFICATENAME = os.environ.get("MicrosoftAppCertificateName", "")
    APP_CERTIFICATETHUMBPRINT = os.environ.get("MicrosoftAppCertificateThumbprint", "")
