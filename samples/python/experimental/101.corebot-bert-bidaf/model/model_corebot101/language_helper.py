# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Language helper that invokes the language model.
This is used from the Bot and Model Runtime to load and invoke the language models.
"""

import os
import sys
from typing import Dict
from pathlib import Path
import requests
from datatypes_date_time.timex import Timex
from model_corebot101.booking_details import BookingDetails
from model_corebot101.bidaf.model_runtime import BidafModelRuntime
from model_corebot101.bert.model_runtime import BertModelRuntime
from model_corebot101.bert.train import BertTrainEval

# pylint:disable=line-too-long
class LanguageHelper:
    """Language helper that invokes the language model."""

    home_dir = str(Path.home())
    bert_model_dir_default = os.path.abspath(os.path.join(home_dir, "models/bert"))
    bidaf_model_dir_default = os.path.abspath(os.path.join(home_dir, "models/bidaf"))

    # pylint:disable=bad-continuation
    def __init__(self):
        """Create Language Helper.
        Note: Creating the Bert/Bidaf Model Runtime is only necessary for in-proc usage.
        """
        self._bidaf_entities = None
        self._bert_intents = None

    @property
    def entities(self) -> BidafModelRuntime:
        """Model used to detect entities."""
        return self._bidaf_entities

    @property
    def intents(self) -> BertModelRuntime:
        """Model used to detect intents."""
        return self._bert_intents

    def initialize_models(
        self,
        bert_model_dir: str = bert_model_dir_default,
        bidaf_model_dir: str = bidaf_model_dir_default,
    ) -> bool:
        """ Initialize models.
        Perform initialization of the models.
        """
        if not BidafModelRuntime.init_bidaf(bidaf_model_dir, download_ntlk_punkt=True):
            print(
                f"bidaf model creation failed at model directory {bidaf_model_dir}..",
                file=sys.stderr,
            )
            return False

        if not BertModelRuntime.init_bert(bert_model_dir):
            print(
                "bert model creation failed at model directory {bert_model_dir}..",
                file=sys.stderr,
            )
            return False

        print(f"Loading BERT model from {bert_model_dir}...", file=sys.stderr)
        if not os.listdir(bert_model_dir):
            print(f"No BERT model present, building model..", file=sys.stderr)
            BertTrainEval.train_eval(cleanup_output_dir=True)

        self._bert_intents = BertModelRuntime(
            model_dir=bert_model_dir, label_list=["Book flight", "Cancel"]
        )
        print(f"Loaded BERT model.  Loading BiDaf model..", file=sys.stderr)

        self._bidaf_entities = BidafModelRuntime(
            targets=["from", "to", "date"],
            queries={
                "from": "which city will you travel from?",
                "to": "which city will you travel to?",
                "date": "which date will you travel?",
            },
            model_dir=bidaf_model_dir,
        )
        print(f"Loaded BiDAF model from {bidaf_model_dir}.", file=sys.stderr)

        return True

    async def excecute_query_inproc(self, utterance: str) -> BookingDetails:
        """Exeecute a query against language model."""
        booking_details = BookingDetails()
        intent = self.intents.serve(utterance)
        print(f'Recognized intent "{intent}" from "{utterance}".', file=sys.stderr)
        if intent == "Book flight":
            # Bert gave us the intent.
            # Now look for entities with BiDAF..
            entities = self.entities.serve(utterance)

            if "to" in entities:
                print(f'   Recognized "to" entitiy: {entities["to"]}.', file=sys.stderr)
                booking_details.destination = entities["to"]
            if "from" in entities:
                print(
                    f'   Recognized "from" entitiy: {entities["from"]}.',
                    file=sys.stderr,
                )
                booking_details.origin = entities["from"]
            if "date" in entities:
                # This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
                # TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
                print(
                    f'   Recognized "date" entitiy: {entities["date"]}.',
                    file=sys.stderr,
                )
                travel_date = entities["date"]
                if await LanguageHelper.validate_timex(travel_date):
                    booking_details.travel_date = travel_date

        return booking_details

    @staticmethod
    async def excecute_query_service(
        configuration: dict, utterance: str
    ) -> BookingDetails:
        """Invoke lu service to perform prediction/evaluation of utterance."""
        booking_details = BookingDetails()
        lu_response = await LanguageHelper.call_model_runtime(configuration, utterance)
        if lu_response.status_code == 200:

            response_json = lu_response.json()
            intent = response_json["intent"] if "intent" in response_json else None
            entities = await LanguageHelper.validate_entities(
                response_json["entities"] if "entities" in response_json else None
            )
            if intent:
                if "to" in entities:
                    print(
                        f'   Recognized "to" entity: {entities["to"]}.', file=sys.stderr
                    )
                    booking_details.destination = entities["to"]
                if "from" in entities:
                    print(
                        f'   Recognized "from" entity: {entities["from"]}.',
                        file=sys.stderr,
                    )
                    booking_details.origin = entities["from"]
                if "date" in entities:
                    # This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
                    # TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
                    print(
                        f'   Recognized "date" entity: {entities["date"]}.',
                        file=sys.stderr,
                    )
                    travel_date = entities["date"]
                    if await LanguageHelper.validate_timex(travel_date):
                        booking_details.travel_date = travel_date
        return booking_details

    @staticmethod
    async def call_model_runtime(
        configuration: Dict[str, object], text: str
    ) -> requests.Request:
        """ Makes a call to the model runtime api

        The model runtime api signature is:
          http://<model_runtime_host>:<port>/v1.0/model?q=<text>

        where:

          model_runtime_host - The host running the model runtime api.  To resolve
            the host running the model runtime api (in the following order):
            - MODEL_RUNTIME_API environment variable.  Used in docker.
            - config.py (which contains the DefaultConfig class).  Used running
                locally.

          port - http port number (ie, 8880)

          q - A query string to process (ie, the text utterance from user)

        For more details: (See TBD swagger file)
        """
        port = os.environ.get("MODEL_RUNTIME_SERVICE_PORT")
        host = os.environ.get("MODEL_RUNTIME_SERVICE_HOST")
        if host is None:
            host = configuration["MODEL_RUNTIME_SERVICE_HOST"]
        if port is None:
            port = configuration["MODEL_RUNTIME_SERVICE_PORT"]

        api_url = f"http://{host}:{port}/v1.0/model"
        qstrings = {"q": text}
        return requests.get(api_url, params=qstrings)

    @staticmethod
    async def validate_entities(entities: Dict[str, str]) -> bool:
        """Validate the entities.
        The to and from cities can't be the same.  If this is detected,
        remove the ambiguous results. """
        if "to" in entities and "from" in entities:
            if entities["to"] == entities["from"]:
                del entities["to"]
                del entities["from"]
        return entities

    @staticmethod
    async def validate_timex(travel_date: str) -> bool:
        """Validate the time.
        Make sure time given in the right format. """
        # uncomment the following line for debugging.
        # import pdb; pdb.set_trace()
        timex_property = Timex(travel_date)

        return len(timex_property.types) > 0 and "definite" not in timex_property.types
