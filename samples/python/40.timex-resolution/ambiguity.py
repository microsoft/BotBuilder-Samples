# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from recognizers_date_time import recognize_datetime, Culture


class Ambiguity:
    """
    TIMEX expressions are designed to represent ambiguous rather than definite dates. For
    example: "Monday" could be any Monday ever. "May 5th" could be any one of the possible May
    5th in the past or the future. TIMEX does not represent ambiguous times. So if the natural
    language mentioned 4 o'clock it could be either 4AM or 4PM. For that the recognizer (and by
    extension LUIS) would return two TIMEX expressions. A TIMEX expression can include a date and
    time parts. So ambiguity of date can be combined with multiple results. Code that deals with
    TIMEX expressions is frequently dealing with sets of TIMEX expressions.
    """

    @staticmethod
    def date_ambiguity():
        # Run the recognizer.
        results = recognize_datetime(
            "Either Saturday or Sunday would work.", Culture.English
        )

        # We should find two results in this example.
        for result in results:
            # The resolution includes two example values: going backwards and forwards from NOW in the calendar.
            # Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
            # We are interested in the distinct set of TIMEX expressions.
            # There is also either a "value" property on each value or "start" and "end".
            distinct_timex_expressions = {
                value["timex"]
                for value in result.resolution["values"]
                if "timex" in value
            }
            print(f"{result.text} ({','.join(distinct_timex_expressions)})")

    @staticmethod
    def time_ambiguity():
        # Run the recognizer.
        results = recognize_datetime(
            "We would like to arrive at 4 o'clock or 5 o'clock.", Culture.English
        )

        # We should find two results in this example.
        for result in results:
            # The resolution includes two example values: one for AM and one for PM.
            # Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
            #  We are interested in the distinct set of TIMEX expressions.
            distinct_timex_expressions = {
                value["timex"]
                for value in result.resolution["values"]
                if "timex" in value
            }

            # TIMEX expressions don't capture time ambiguity so there will be two distinct expressions for each result.
            print(f"{result.text} ({','.join(distinct_timex_expressions)})")

    @staticmethod
    def date_time_ambiguity():
        # Run the recognizer.
        results = recognize_datetime(
            "It will be ready Wednesday at 5 o'clock.", Culture.English
        )

        # We should find a single result in this example.
        for result in results:
            # The resolution includes four example values: backwards and forward in the calendar and then AM and PM.
            # Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
            # We are interested in the distinct set of TIMEX expressions.
            distinct_timex_expressions = {
                value["timex"]
                for value in result.resolution["values"]
                if "timex" in value
            }

            # TIMEX expressions don't capture time ambiguity so there will be two distinct expressions for each result.
            print(f"{result.text} ({','.join(distinct_timex_expressions)})")
