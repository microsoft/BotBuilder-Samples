# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from recognizers_date_time import recognize_datetime
from recognizers_text import Culture


class Ranges:
    """
    TIMEX expressions can represent date and time ranges. Here are a couple of examples.
    """

    @staticmethod
    def date_range():
        # Run the recognizer.
        results = recognize_datetime(
            "Some time in the next two weeks.", Culture.English
        )

        # We should find a single result in this example.
        for result in results:
            # The resolution includes a single value because there is no ambiguity.
            # We are interested in the distinct set of TIMEX expressions.
            distinct_timex_expressions = {
                value["timex"]
                for value in result.resolution["values"]
                if "timex" in value
            }

            # The TIMEX expression can also capture the notion of range.
            print(f"{result.text} ({','.join(distinct_timex_expressions)})")

    @staticmethod
    def time_range():
        # Run the recognizer.
        results = recognize_datetime(
            "Some time between 6pm and 6:30pm.", Culture.English
        )

        # We should find a single result in this example.
        for result in results:
            # The resolution includes a single value because there is no ambiguity.
            # We are interested in the distinct set of TIMEX expressions.
            distinct_timex_expressions = {
                value["timex"]
                for value in result.resolution["values"]
                if "timex" in value
            }

            # The TIMEX expression can also capture the notion of range.
            print(f"{result.text} ({','.join(distinct_timex_expressions)})")
