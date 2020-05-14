#!/usr/bin/env python
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

""" URL configuration for bot message handler """

from django.urls import path
from django.views.decorators.csrf import csrf_exempt
from . import views

# pylint:disable=invalid-name
urlpatterns = [
    path("", views.home, name="home"),
    path("api/messages", csrf_exempt(views.messages), name="messages"),
]
