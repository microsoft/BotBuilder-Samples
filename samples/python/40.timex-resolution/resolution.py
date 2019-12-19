# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import datetime

from datatypes_timex_expression import TimexResolver


class Resolution:
    """
    Given the TIMEX expressions it is easy to create the computed example values that the recognizer gives.
    """

    @staticmethod
    def examples():
        # When you give the recognizer the text "Wednesday 4 o'clock" you get these distinct TIMEX values back.

        today = datetime.datetime.now()
        resolution = TimexResolver.resolve(["XXXX-WXX-3T04", "XXXX-WXX-3T16"], today)

        print(f"Resolution Values: {len(resolution.values)}")

        for value in resolution.values:
            print(value.timex)
            print(value.type)
            print(value.value)
