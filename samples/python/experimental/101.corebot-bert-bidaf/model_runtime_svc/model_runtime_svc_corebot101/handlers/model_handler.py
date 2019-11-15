# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Tornado handler to access the model runtime.

To invoke:
  /v1.0/model?q=<text>
"""

import logging
import json
from tornado.web import RequestHandler
from model_corebot101.language_helper import LanguageHelper

# pylint:disable=abstract-method
class ModelHandler(RequestHandler):
    """Model Handler implementation to access the model runtime."""

    _handler_routes = ["/v1.0/model/$", "/v1.0/model$"]

    @classmethod
    def build_config(cls, ref_obj: dict):
        """Build the Tornado configuration for this handler."""
        return [(route, ModelHandler, ref_obj) for route in cls._handler_routes]

    def set_default_headers(self):
        """Set the default HTTP headers."""
        RequestHandler.set_default_headers(self)
        self.set_header("Content-Type", "application/json")
        self.set_header("Access-Control-Allow-Origin", "*")
        self.set_header("Access-Control-Allow-Headers", "Origin, Content-Type, Accept")
        self.set_header("Access-Control-Allow-Methods", "OPTIONS, GET")

    # pylint:disable=attribute-defined-outside-init
    def initialize(self, language_helper: LanguageHelper):
        """Initialize the handler."""
        RequestHandler.initialize(self)
        self._language_helper = language_helper
        self._logger = logging.getLogger("MODEL_HANDLER")

    async def get(self):
        """Handle HTTP GET request."""
        text = self.get_argument("q", None, True)
        if not text:
            return (404, "Missing the q query string with the text")

        response = {}
        intent = self._language_helper.intents.serve(text)
        response["intent"] = intent if intent else "None"
        entities = self._language_helper.entities.serve(text)
        response["entities"] = entities if entities else "None"
        self.write(json.dumps(response))
        return (200, "Complete")
