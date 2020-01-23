# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import datetime

from datatypes_timex_expression import TimexRangeResolver, TimexCreator


class Constraints:
    """
    The TimexRangeResolved can be used in application logic to apply constraints to a set of TIMEX expressions.
    The constraints themselves are TIMEX expressions. This is designed to appear a little like a database join,
    of course its a little less generic than that because dates can be complicated things.
    """

    @staticmethod
    def examples():
        """
        When you give the recognizer the text "Wednesday 4 o'clock" you get these distinct TIMEX values back.
        But our bot logic knows that whatever the user says it should be evaluated against the constraints of
        a week from today with respect to the date part and in the evening with respect to the time part.
        """

        resolutions = TimexRangeResolver.evaluate(
            ["XXXX-WXX-3T04", "XXXX-WXX-3T16"],
            [TimexCreator.week_from_today(), TimexCreator.EVENING],
        )

        today = datetime.datetime.now()
        for resolution in resolutions:
            print(resolution.to_natural_language(today))
