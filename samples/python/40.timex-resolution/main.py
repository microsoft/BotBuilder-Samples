# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from ambiguity import Ambiguity
from constraints import Constraints
from language_generation import LanguageGeneration
from parsing import Parsing
from ranges import Ranges
from resolution import Resolution

if __name__ == "__main__":
    # Creating TIMEX expressions from natural language using the Recognizer package.
    Ambiguity.date_ambiguity()
    Ambiguity.time_ambiguity()
    Ambiguity.date_time_ambiguity()
    Ranges.date_range()
    Ranges.time_range()

    # Manipulating TIMEX expressions in code using the TIMEX Datatype package.
    Parsing.examples()
    LanguageGeneration.examples()
    Resolution.examples()
    Constraints.examples()
