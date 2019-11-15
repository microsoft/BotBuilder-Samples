# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
"""Model Cache.
Simple container for bidaf/bert models.
"""
import os
import logging

from model_corebot101.bidaf.model_runtime import BidafModelRuntime
from model_corebot101.bert.model_runtime import BertModelRuntime

# pylint:disable=line-too-long,bad-continuation
class DeprecateModelCache(object):
    """Model Cache implementation."""

    def __init__(self):
        self._logger = logging.getLogger("ModelCache")
        self._bert_model_dir = None
        self._bidaf_model_dir = None
        self._bert_intents = None
        self._bidaf_entities = None

    def init_model_dir(self, bidaf_model_dir: str, bert_model_dir: str) -> bool:
        """ Initialize models """
        if not os.path.exists(bidaf_model_dir):
            # BiDAF needs no training, just download
            if not BidafModelRuntime.init_bidaf(bidaf_model_dir, True):
                self._logger.error(
                    "bidaf model creation failed at model directory %s..",
                    bidaf_model_dir,
                )
                return False

        if not os.path.exists(bert_model_dir):
            self._logger.error(
                'BERT model directory does not exist "%s"', bert_model_dir
            )
            return False

        self._bert_model_dir = os.path.normpath(bert_model_dir)
        self._bidaf_model_dir = os.path.normpath(bidaf_model_dir)

        self._bert_intents = BertModelRuntime(
            model_dir=self._bert_model_dir, label_list=["Book flight", "Cancel"]
        )
        self._bidaf_entities = BidafModelRuntime(
            targets=["from", "to", "date"],
            queries={
                "from": "which city will you travel from?",
                "to": "which city will you travel to?",
                "date": "which date will you travel?",
            },
            model_dir=self._bidaf_model_dir,
        )
        self._logger.info("bidaf entities model created : %s..", self._bidaf_model_dir)

        return True

    @property
    def entities(self):
        """Get the model that detect entities: bidaf."""
        return self._bidaf_entities

    @property
    def intents(self):
        """Get the model that detect intents: bert."""
        return self._bert_intents
