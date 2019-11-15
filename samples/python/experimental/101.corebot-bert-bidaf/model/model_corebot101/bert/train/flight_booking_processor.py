# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import json
import os
from typing import List, Tuple

from model_corebot101.bert.common.input_example import InputExample


class FlightBookingProcessor:
    """Processor for the flight booking data set."""

    def get_train_examples(self, data_dir):
        """See base class."""
        return self._create_examples(
            self._read_json(os.path.join(data_dir, "FlightBooking.json")), "train"
        )

    def get_dev_examples(self, data_dir):
        """See base class."""
        return self._create_examples(
            self._read_json(os.path.join(data_dir, "FlightBooking.json")), "dev"
        )

    def get_labels(self):
        """See base class."""
        return ["Book flight", "Cancel"]

    def _create_examples(self, lines, set_type):
        """Creates examples for the training and dev sets."""
        examples = []
        for (i, line) in enumerate(lines):
            guid = "%s-%s" % (set_type, i)
            text_a = line[1]
            label = line[0]
            examples.append(
                InputExample(guid=guid, text_a=text_a, text_b=None, label=label)
            )
        return examples

    @classmethod
    def _read_json(cls, input_file):
        with open(input_file, "r", encoding="utf-8") as f:
            obj = json.load(f)
            examples = obj["utterances"]
            lines: List[Tuple[str, str]] = []
            for example in examples:
                lines.append((example["intent"], example["text"]))

            return lines
