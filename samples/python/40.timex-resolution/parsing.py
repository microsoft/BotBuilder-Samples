# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from datatypes_timex_expression import Timex, Constants


class Parsing:
    """
    The Timex class takes a TIMEX expression as a string argument in its constructor.
    This pulls all the component parts of the expression into properties on this object. You can
    then manipulate the TIMEX expression via those properties.
    The "types" property infers a datetimeV2 type from the underlying set of properties.
    If you take a TIMEX with date components and add time components you add the
    inferred type datetime (its still a date).
    Logic can be written against the inferred type, perhaps to have the bot ask the user for
    disambiguation.
    """

    @staticmethod
    def __describe(timex_pattern: str):
        timex = Timex(timex_pattern)

        print(timex.timex_value(), end=" ")

        if Constants.TIMEX_TYPES_DATE in timex.types:
            if Constants.TIMEX_TYPES_DEFINITE in timex.types:
                print("We have a definite calendar date.", end=" ")
            else:
                print("We have a date but there is some ambiguity.", end=" ")

        if Constants.TIMEX_TYPES_TIME in timex.types:
            print("We have a time.")
        else:
            print("")

    @staticmethod
    def examples():
        """
        Print information an various TimeX expressions.
        :return: None
        """
        Parsing.__describe("2017-05-29")
        Parsing.__describe("XXXX-WXX-6")
        Parsing.__describe("XXXX-WXX-6T16")
        Parsing.__describe("T12")
