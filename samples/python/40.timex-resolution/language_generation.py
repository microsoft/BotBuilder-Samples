# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import datetime

from datatypes_timex_expression import Timex


class LanguageGeneration:
    """
    This language generation capabilities are the logical opposite of what the recognizer does.
    As an experiment try feeding the result of language generation back into a recognizer.
    You should get back the same TIMEX expression in the result.
    """

    @staticmethod
    def examples():
        LanguageGeneration.__describe(Timex("2019-05-29"))
        LanguageGeneration.__describe(Timex("XXXX-WXX-6"))
        LanguageGeneration.__describe(Timex("XXXX-WXX-6T16"))
        LanguageGeneration.__describe(Timex("T12"))

        LanguageGeneration.__describe(Timex.from_date(datetime.datetime.now()))
        LanguageGeneration.__describe(
            Timex.from_date(datetime.datetime.now() + datetime.timedelta(days=1))
        )

    @staticmethod
    def __describe(timex: Timex):
        # Note natural language is often relative, for example the sentence "Yesterday all my troubles seemed so far
        # away." Having your bot say something like "next Wednesday" in a response can make it sound more natural.
        reference_date = datetime.datetime.now()
        print(f"{timex.timex_value()} : {timex.to_natural_language(reference_date)}")
