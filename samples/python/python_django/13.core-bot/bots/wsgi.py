#!/usr/bin/env python
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

"""
WSGI config for bots project.

It exposes the WSGI callable as a module-level variable named ``application``.

For more information on this file, see
https://docs.djangoproject.com/en/2.2/howto/deployment/wsgi/
"""

import os
from django.core.wsgi import get_wsgi_application

# pylint:disable=invalid-name
os.environ.setdefault("DJANGO_SETTINGS_MODULE", "bots.settings")
application = get_wsgi_application()
